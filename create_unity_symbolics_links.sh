#!/bin/bash

# Creation du lien symbolique du dossier Assets

# Suppression du dossier Assets du nouveau projet

rm -rf UnityProject/Assets

# Creation des liens symboliques sur le dossiers Assets 

ln -s ../Unity-Assets UnityProject/Assets
