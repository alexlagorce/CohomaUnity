#!/bin/bash

# Installation de Unity Hub sur Linux ( Debian or Ubuntu )

# Ajout de la clé de signature publique

wget -qO - https://hub.unity3d.com/linux/keys/public | gpg --dearmor | sudo tee /usr/share/keyrings/Unity_Technologies_ApS.gpg > /dev/null

# Ajout du dépôt Unity Hub

sudo sh -c 'echo "deb [signed-by=/usr/share/keyrings/Unity_Technologies_ApS.gpg] https://hub.unity3d.com/linux/repos/deb stable main" > /etc/apt/sources.list.d/unityhub.list'

# Installation de Unity Hub

sudo apt update
sudo apt-get install -y unityhub