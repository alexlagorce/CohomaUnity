#!/bin/bash

# Creation d'un nouveau projet Unity

~/Unity-2021.3.33f1/Editor/Unity  -createProject UnityProject

# Suppression des dossiers Assets et Packages du nouveau projet

rm -rf CohomaProject/Assets
rm -rf CohomaProject/Packages

# Creation des liens symboliques sur les dossiers Assets et Package

ln -s ../Unity-Assets CohomaProject/Assets
ln -s '../PICO Unity Integration SDK 240' 'CohomaProject/PICO Unity Integration SDK 240'
ln -s '../PICO Unity Live Preview Plugin (Experiment)' 'CohomaProject/PICO Unity Live Preview Plugin (Experiment)'
