# -*- coding: utf-8 -*-
import sys
import joblib

# Récupère le texte des symptômes passé en argument
texte = sys.argv[1]
currentPath = sys.argv[2]

# Charge le modèle
model = joblib.load(f"{currentPath}/modele/modele_alzheimer1.joblib")

# Fait la prédiction
pred = model.predict([texte])[0]
confiance = model.predict_proba([texte])[0][pred]
label = "Alzheimer" if pred == 1 else "Non"

# Affiche le résultat pour que .NET puisse le lire
print(f"{label}|{confiance:.2f}")
