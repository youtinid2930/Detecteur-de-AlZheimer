import numpy as np
import tensorflow as tf
from tensorflow import keras
from PIL import Image
import io
import pydicom
import json
import logging
import os


logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("AlzheimerPredictor")

class AlzheimerPredictor:
    def __init__(self, model_path: str, classes_path: str):
        try:

            if not os.path.exists(model_path):
                raise FileNotFoundError(f"Fichier modele introuvable: {model_path}")
            if not os.path.exists(classes_path):
                raise FileNotFoundError(f"Fichier classes introuvable: {classes_path}")
            
           
            logger.info(f"Chargement du modele depuis {model_path}...")
            self.model = keras.models.load_model(model_path)
            
            
            if not self.model.input_shape:
                raise ValueError("Forme d'entree du modele non definie")
            
            
            self.IMG_HEIGHT = self.model.input_shape[1]
            self.IMG_WIDTH = self.model.input_shape[2]
            logger.info(f"Taille d entree detectee: {self.IMG_WIDTH}x{self.IMG_HEIGHT}")
            
           
            logger.info(f"Chargement des classes depuis {classes_path}...")
            with open(classes_path, 'r') as f:
                self.classes = json.load(f)
            
            logger.info("Modele et classes charges avec succes!")
            
        except Exception as e:
            logger.error(f"Erreur lors du chargement: {str(e)}")
            raise

    def preprocess_image(self, file_bytes: bytes, is_dicom: bool = False) -> np.ndarray:
        try:
            
            if is_dicom:
                ds = pydicom.dcmread(io.BytesIO(file_bytes))
                img_array = ds.pixel_array.astype(float)
                
               
                img_array = (img_array - np.min(img_array)) / np.ptp(img_array) * 255
                img = Image.fromarray(img_array.astype(np.uint8)).convert('RGB')
            else:
                img = Image.open(io.BytesIO(file_bytes)).convert('RGB')
            
           
            img = img.resize((self.IMG_WIDTH, self.IMG_HEIGHT))
            img_array = np.array(img)
            
           
            return np.expand_dims(img_array, axis=0)
            
        except Exception as e:
            logger.error(f"Erreur de pretraitement: {str(e)}")
            raise

    def predict(self, file_bytes: bytes, content_type: str) -> dict:
        try:
           
            is_dicom = content_type == 'application/dicom'
            
            
            input_array = self.preprocess_image(file_bytes, is_dicom)
            
            
            logger.info("Execution de la prediction...")
            prediction = self.model.predict(input_array, verbose=0)
            class_idx = np.argmax(prediction[0])
            max_prob = float(np.max(prediction[0]))
           

            probabilities = {
                self.classes[str(i)]: float(prob) 
                for i, prob in enumerate(prediction[0])
            }
            
            return {
                "class": self.classes[str(class_idx)],
                "confidence": max_prob,
                "probabilities": probabilities,
                "class_index": int(class_idx)
            }
            
        except Exception as e:
            logger.error(f"Erreur de prediction: {str(e)}")
            raise


if __name__ == "__main__":
    BASE_DIR = os.path.dirname(os.path.abspath(__file__))
    MODEL_PATH = os.path.join(BASE_DIR, '../models/mymodel.keras')
    CLASSES_PATH = os.path.join(BASE_DIR, '../models/classes.json')
    
    predictor = AlzheimerPredictor(MODEL_PATH, CLASSES_PATH)
    
   
    TEST_IMAGE = os.path.join(BASE_DIR, 'test_image.jpg')
    try:
        with open(TEST_IMAGE, "rb") as f:
            img_bytes = f.read()
        
        result = predictor.predict(img_bytes, "image/jpeg")
        print("\nResultat de test:")
        print(json.dumps(result, indent=2))
    except FileNotFoundError:
        logger.warning(f"Image de test introuvable: {TEST_IMAGE}")