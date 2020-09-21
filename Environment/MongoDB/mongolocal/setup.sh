#!/bin/bash

echo Setting up replset
sleep 20 | echo Sleeping
mongo mongodb://mongo1:28017 replset.js
