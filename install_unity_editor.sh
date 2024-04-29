#!/bin/bash

# Installation de la librairie libssl

# Si libssl n'est pas installé, une erreur critique apparait dans l'éditeur Unity ce qui ne permet pas de lancer le projet.

echo "deb [trusted=yes] http://security.ubuntu.com/ubuntu focal-security main" | sudo tee /etc/apt/sources.list.d/focal-security.list
sudo apt-get update
sudo apt-get install libssl1.1

# Installation de l'éditeur Unity 

# Téléchargement de l'archive Unity ( version 2021.3.33f1 ) 

wget https://download.unity3d.com/download_unity/ee5a2aa03ab2/UnitySetup-2021.3.33f1

# Ajout de l'autorisation d'exécution du fichier 

chmod +x UnitySetup-2021.3.33f1

# Execution du fichier

./UnitySetup-2021.3.33f1