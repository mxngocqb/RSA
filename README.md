# Navigation Data Backend Application - .NET 8

## Overview

This repository contains a backend application developed in .NET 8 for managing navigation message data. The navigation message data comprises essential information utilized in navigation systems. It includes the following fields:

- `SvID`: Satellite Vehicle Identifier.
- `week`: Week number.
- `tow`: Time of Week.
- `navigationMessage`: Navigation message data.
- `signature`: RSA signature for ensuring data integrity and authenticity.

The application leverages RSA signature verification to ensure the integrity and authenticity of the data.

## Technologies Used

- Backend: .NET 8
- Encryption Algorithm: RSA

## Setup

### Clone the Repository

Begin by cloning this repository to your local machine.
```bash
git clone https://github.com/mxngocqb/RSA.git
