#!/bin/bash

# Simple test client for the SauronVisionProtocol server
# Usage: ./test-client.sh [host] [port]

HOST=${1:-localhost}
PORT=${2:-9000}

echo "Connecting to SauronVisionProtocol server at $HOST:$PORT..."

# Use netcat to establish a connection
{ 
    # Wait for welcome message
    sleep 1
    
    # Send a PALANTIR_GAZE command
    echo "PALANTIR_GAZE gondor"
    sleep 1
    
    # Send another command
    echo "PALANTIR_GAZE mordor"
    sleep 1
    
    # Send an invalid command to test error handling
    echo "INVALID_COMMAND"
    sleep 1
    
    # Exit
    echo "exit"
    
} | nc $HOST $PORT

echo "Test completed."
