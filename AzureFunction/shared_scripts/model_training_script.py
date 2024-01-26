import numpy as np
import keras

import cv2, json
from tqdm import *

import requests
import shared_methods as shared

# Models
from keras.applications import Xception

from keras.models import Model
from keras.layers import Dense
from keras.layers import Dropout
from keras.layers import Flatten
from keras.models import Sequential

ml_client = shared.get_ml_client()

api_endpoint = 'https://seaslugsapi.azurewebsites.net/api'
headers = {
    'x-api-key': 'a1491071-4a91-4f23-83b0-1538d647357f',
    'Content-Type': 'application/json'
}

# Get all the slug species
slug_species = []

try:
    response = requests.get(f'{api_endpoint}/sea-slugs/all', headers=headers, verify=True)

    if response.status_code == 200:
        slug_species = response.json()
    else:
        print(f"Request failed: {response.text}")

except requests.RequestException as e:
    print(f"Request failed: {e}")

# Get the training data
try:
    response = requests.get(f'{api_endpoint}/training/training-files', headers=headers, verify=True)

    if response.status_code == 200:
        blob_urls_with_folders = response.json()
        label_names = [url.split('/')[-2] for url in blob_urls_with_folders]
        images = []
        for url in blob_urls_with_folders:
            response = requests.get(url)
            if response.status_code == 200:
                img_array = np.asarray(
                    bytearray(response.content), dtype=np.uint8)
                img = cv2.imdecode(img_array, cv2.IMREAD_COLOR)
                img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
                # img = cv2.resize(img, (128, 128))  # Resize the image to a standard size
                img = cv2.resize(img, None, fx=0.5, fy=0.5,
                                 interpolation=cv2.INTER_AREA)
                images.append(img)
            else:
                print(f"Failed to download image from {url}")

        # Extract 'id' and 'label' pairs from the slug_species
        id_label_pairs = [(obj['id'], obj['label']) for obj in slug_species]


        label_dict = {id_value: label_value for id_value, 
                      label_value in id_label_pairs if id_value in label_names}
        labels = [label_dict[label] for label in label_names]
except requests.RequestException as e:
    print(f"Request failed: {e}")

print('species', slug_species)
print('label_names', label_names)
print('labels', labels)

filtered_images = []
filtered_labels = []

min_images_per_class = 10

label_counts = {label: labels.count(label) for label in set(labels)}
label_warning_printed = {} 

for img, label in zip(images, labels):
    if label_counts[label] >= min_images_per_class:
        filtered_images.append(img)
        filtered_labels.append(label)
    else:
        if label not in label_warning_printed:
            found_slug= next((obj for obj in slug_species if obj['label'] == label), None)
            print(f'Could not train the model on the {found_slug["name"]} species as there are not enough images ({min_images_per_class} required at least).')
            label_warning_printed[label] = True

print('filtered_labels', filtered_labels)

def define_model():
    base_model = Xception(weights='imagenet', include_top=False, input_shape=(250, 250, 3))
    base_model.trainable = False 
    
    dense_model = Sequential()
    dense_model.add(Flatten(input_shape=base_model.output_shape[1:]))
    dense_model.add(Dense(512, activation='relu'))
    dense_model.add(Dropout(0.60))
    dense_model.add(Dense(len(slug_species), activation='softmax'))

    model = Model(inputs=base_model.input, outputs=dense_model(base_model.output))
    
    # Load the weights of the downloaded Azure model
    print('--Downloading model--')
    model_dir = "./downloaded_model"
    ml_client.models.download(name="slug-model-expanded", version=1, download_path=model_dir)
    model.load_weights(f'{model_dir}/slug-model-expanded/Model/variables/variables')
    
    print('--Compiling model--')
    # Compile the model after loading the weights
    model.compile(loss='categorical_crossentropy', optimizer=keras.optimizers.SGD(learning_rate=0.0001, momentum=0.9),
              metrics=['accuracy'])
    
    return model

def save_results(model, history, model_name) :
    # save model
    print('--Saving model--')
    model_path = f'{model_name}'
    model.save(model_path)    
    
    # save history
    with open(f'{model_path}/history.json',"w") as f:
        json.dump(history.history,f)

    azure_model = shared.register_model(ml_client, model_path)

    return azure_model
    
# run the test harness for evaluating a model
def run_test_harness(model_name=None):    
    train_gen = shared.DataGenerator(X_train, y_train_vector, 16)
    val_gen = shared.DataGenerator(X_val, y_val_vector, 16)
    
    model = define_model()
    
    print('\n---Training---')

    hist = model.fit(
        train_gen,
        epochs=5,
        validation_data=val_gen,
        verbose=1
    )
    
    if model_name != None:
        print('\n---Saving Model---')
        azure_model = save_results(model, hist, model_name)
    
    return model, azure_model, hist

if(len(filtered_labels) > 0):
    X_train, X_test, X_val, y_train, y_test, y_val, y_train_vector, y_test_vector, y_val_vector = shared.prepare_data(filtered_images, filtered_labels, len(slug_species))

    model, azure_model, hist = run_test_harness('Model')
    shared.deploy_endpoint(ml_client, azure_model)