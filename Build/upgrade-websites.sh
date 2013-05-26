#!/bin/bash

echo "Stopping service..."
sudo systemctl stop mono-makemeaworld.com.service

echo "Clearing out temporary directories..."
rm -Rf /tmp/*aspnet*

echo "Resetting and cleaning changes..."
git reset --hard HEAD && git clean -xdf

echo "Pulling latest version..."
git pull

echo "Linking back Local.config..."
ln -s ../../Local.config /srv/www/tychaia.com/mmaw/MakeMeAWorld/Local.config

echo "Starting service again..."
sudo systemctl start mono-makemeaworld.com.service
