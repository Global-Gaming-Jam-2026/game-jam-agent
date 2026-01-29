#!/bin/bash
# Post-edit hook - runs after file edits
# Logs changes for tracking

LOGFILE="./docs/edit_log.txt"
echo "[$(date '+%Y-%m-%d %H:%M:%S')] File edited" >> "$LOGFILE"
