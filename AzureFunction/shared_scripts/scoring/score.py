import json
import base64
import io
from PIL import Image
import numpy as np
import tensorflow as tf

def init():
    # Retrieve and load the model
    global model

    # The name of the model
    model_name = 'sea-slug-identification-model'
    # The version of the model
    model_version = 1
    # The name of the folder that was uploaded during the model registration
    model_folder = 'Model'

    model_path = '/var/azureml-app/azureml-models/' + model_name + '/' + str(model_version) + '/' + model_folder
    model = tf.keras.models.load_model(model_path)

def preprocess_image(image):
    if image.mode != "RGB":
        image = image.convert("RGB")
    image = image.resize((250, 250))
    image = np.array(image).astype('float32') / 255.0
    image = np.expand_dims(image, axis=0)
    return image

def run(raw_data):
    print(raw_data)
    try:    
        decoded = base64.b64decode(raw_data)
        image = Image.open(io.BytesIO(decoded))
        processed_image = preprocess_image(image)

        # Get prediction
        prediction = model.predict(processed_image)

        # Get the prediction probabilities
        # Get the top 3 predicted labels and their corresponding probabilities
        top_3_prediction_labels = np.argsort(prediction[0])[::-1][:3]
        top_3_probabilities = prediction[0, top_3_prediction_labels]

        # Group the labels and probabilities for the response
        response = [(int(label), float(probability)) for label, probability in zip(top_3_prediction_labels, top_3_probabilities)]
        # Convert the list of tuples to a dictionary
        result_dict = [{"label": int(label), "probability": float(probability)} for label, probability in response]

        return {'data' : json.dumps(result_dict) , 'message' : "Successfully classified slug"}

    except Exception as e:
        error = str(e)
        return {'data' : json.dumps([]) , 'message' : error}