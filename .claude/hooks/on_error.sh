#!/bin/bash
# Error hook - runs when tool errors occur
# Logs errors for debugging

LOGFILE="./docs/error_log.txt"
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Error occurred" >> "$LOGFILE"
