# azure
from azure.identity import ClientSecretCredential
from azure.ai.ml import MLClient
from azure.ai.ml.entities import Model, ManagedOnlineEndpoint, CodeConfiguration, ManagedOnlineDeployment

# Processing
from sklearn.model_selection import train_test_split
from keras.utils import Sequence, to_categorical

import numpy as np

import cv2
from tqdm import *

# Training methods
class DataGenerator(Sequence):
    def __init__(self, x_set, y_set, batch_size):
        self.x, self.y = x_set, y_set
        self.batch_size = batch_size

    def __len__(self):
        return int(np.ceil(len(self.x) / float(self.batch_size)))

    def __getitem__(self, idx):
        batch_x = self.x[idx * self.batch_size:(idx + 1) * self.batch_size]
        batch_y = self.y[idx * self.batch_size:(idx + 1) * self.batch_size]
        return batch_x, batch_y

class TestDataGenerator(Sequence):
    def __init__(self, x_set, batch_size):
        self.x = x_set
        self.batch_size = batch_size

    def __len__(self):
        return int(np.ceil(len(self.x) / float(self.batch_size)))

    def __getitem__(self, idx):
        batch_x = self.x[idx * self.batch_size:(idx + 1) * self.batch_size]
        return batch_x

# Helper function to resize image while keeping aspect ratio
def resize_image(img, width, height):
    # Resize
    border_v = 0
    border_h = 0
    if (height/width) >= (img.shape[0]/img.shape[1]):
        border_v = int((((height/width)*img.shape[1])-img.shape[0])/2)
    else:
        border_h = int((((width/height)*img.shape[0])-img.shape[1])/2)
    img = cv2.copyMakeBorder(img, border_v, border_v, border_h, border_h, cv2.BORDER_CONSTANT, 0)
    img = cv2.resize(img, (width, height))
    
    return img

def resize_images(images, width, height):
    resized_images = []
    
    with tqdm(total=len(images)) as k:  
        for img in images:
            img = resize_image(img, width, height)
            resized_images.append(img)
            k.update(1)
    
    resized_images = np.array(resized_images)
    return resized_images

def scale_data(images):
    images.astype('float64')
    scaled_images = images / 255
    return scaled_images

def train_test_val_split(df_X, df_y):
    # 70% Train, 30% Test
    X_train, X_test, y_train, y_test = train_test_split(df_X, df_y, stratify=df_y, test_size=0.30, random_state=42)

    # 15% Test and 15% Validation
    X_val, X_test, y_val, y_test = train_test_split(X_test, y_test, stratify=y_test, test_size=0.5, random_state=42)
    
    return X_train, X_test, X_val, y_train, y_test, y_val

def vectorise_labels(y_train, y_test, y_val, num_classes):
    y_train_vector = to_categorical(y_train, num_classes=num_classes)
    y_test_vector = to_categorical(y_test, num_classes=num_classes)
    y_val_vector = to_categorical(y_val, num_classes=num_classes)
    
    return y_train_vector, y_test_vector, y_val_vector

def prepare_data(images, labels, labels_count):
    resized_images = resize_images(images, 250, 250)
    scaled_images = scale_data(resized_images)
    
    X_train, X_test, X_val, y_train, y_test, y_val = train_test_val_split(scaled_images, labels)
    y_train_vector, y_test_vector, y_val_vector = vectorise_labels(y_train, y_test, y_val, labels_count)
    
    return X_train, X_test, X_val, y_train, y_test, y_val, y_train_vector, y_test_vector, y_val_vector

# Azure methods
def get_ml_client():
    tenant_id = "c66b6765-b794-4a2b-84ed-845b341c086a"
    service_principal_id = "44092769-7233-44f3-9878-fbda9b49b0fc"
    service_principal_password = "rOS8Q~2OQTvfm6a4He.0sewNbWs1KggVkqI-PaVA"
    workspace="sea-slug-wp"
    subscription_id="7c5d4dd1-4815-4ab4-8e58-52b007be1b9b"
    resource_group="sea-slugs-rg"

    # Get the proper credential using the service  principal
    credential = ClientSecretCredential(
        tenant_id=tenant_id,
        client_id=service_principal_id,
        client_secret=service_principal_password
    )

    ml_client = MLClient(
        credential, subscription_id, resource_group, workspace
    )

    ml_client.models

    return ml_client

def register_model(ml_client, model_path):
    print('--Registering model--')

    model = Model(
        name="slug-model-expanded",
        path=model_path
    )
    registered_model = ml_client.models.create_or_update(model)

    return registered_model


def deploy_endpoint(ml_client, model):
    print('--Creating endpoint--')
    # Get environment 
    environment = ml_client.environments.get(name= "Sea-Slug-Env", version=5)

    code_config = CodeConfiguration(code="./scoring", scoring_script="score.py")

    online_endpoint_name = "sea-slug-endpoint"

    endpoint = ManagedOnlineEndpoint(
        name=online_endpoint_name,
        auth_mode="key"
    )

    ml_client.begin_create_or_update(endpoint)

    model_deployment = ManagedOnlineDeployment(
        name="deployment",
        endpoint_name=online_endpoint_name,
        model=model,
        environment=environment,
        code_configuration=code_config,
        instance_type="Standard_D2as_v4",
        instance_count=1
    )

    ml_client.begin_create_or_update(model_deployment)