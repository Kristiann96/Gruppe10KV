# Gruppe10KVprototype

Welcome to the Gruppe10KV prototype repository! 
This README serves as a quick guide to understanding, setting up, and working with the code within this repository.

---

## Table of Contents

- [Overview](#overview)
- [Setup Instructions](#setup-instructions)
- [Repository Structure](#repository-structure)
  - [Models](#models)
  - [Interfaces](#interfaces)
  - [Data Access](#data-access)
- [Technologies Used](#technologies-used)
- [Collaborators](#collaborators)
- [License](#license)

---

## Overview
The application "Gruppe10KVprototype" is based on a project from Kartverket, aiming to facilitate user reporting and handling of geospatial data. It provides a user-friendly interface for submitting reports with geolocation data and ensures efficient handling and review of these reports by system administrators.

### What the Application Does

- User Reporting: Users can submit reports and errors about geographical locations, including geospatial data (GeoJSON format).
- Admin Review: Admin users ("saksbehandlere") can review, assess, and manage these reports efficiently.
- Data Compilation: The system provides a detailed overview and compilation of various geospatial and user-submitted data.
- Authentication and User Management: It features a user authentication system for different types of users, including guest reporters and registered users.

---
<br><br>

## Setup Instructions

Clone the Repository:

    git clone https://github.com/Kristiann96/Gruppe10KV.git


Navigate to the Project Directory:

    cd Gruppe10KV 

Dependencies: Ensure you have all the necessary libraries and runtime environments set up. The repository primarily depends on .NET.

Build the Project: You can build the project using your preferred .NET build commands or using an IDE like Visual Studio.

Configuration: Adjust configuration settings as required. Ensure the database connection strings and other pertinent settings are correctly configured for your environment.

---
<br><br>

## Repository Structure

Below is an overview of the key components of the repository:


### Models

The Models directory is structured to keep entities and different model classes primarily used for data representation.
Below is a simple explanation of one of our models:

  **GeometriModel.cs:**
  
    Contains the Geometri class which represents geometric data.
    Properties include GeometriId, InnmeldingId, and GeometriGeoJson.

<br><br>

### Interfaces:
The Interfaces directory contains all interface definitions for repository patterns, facilitating data operations and abstractions.

  **IGeometriRepository.cs:**
    
    Interface for geometry-related operations.
    Methods for fetching all geometries, getting geometries by innmeldingId, etc.

  **IInnmeldingRepository.cs:** 

    Interface for managing "innmeldinger".
    Methods for fetching and updating "innmeldinger", getting enum values, and more.

  **IVurderingRepository.cs:**
    
    Interface for handling assessments.
    Methods for adding and retrieving evaluations.

  **IGjesteinnmelderRepository.cs:**
  
    Interface for "gjesteinnmeldere".
    Method for creating a "gjesteinnmelder" entry.

  **IInnmelderRepository.cs:**
    
    Interface for registered "innmeldere".
    Methods for validating and fetching "innmelder" information.

  **IDataSammenstillingSaksBRepository.cs:**
    
    Interface for data compilation.
    Methods for getting detailed reports and paginated overviews.

  **ISaksbehandlerRepository.cs:** 
  
    Interface for "saksbehandler".
    Methods for validating and fetching "saksbehandler" information.

  **ILogginnLogic.cs:**
  
    Interface for authentication processes.
    Deals with user authentication logic.

  **ITransaksjonsRepository.cs:**

    Interface for handling transaction-related operations.
    Includes methods for complete report saving, creating, and deleting persons and reporters.

<br><br>

### Data Access

The DataAccess directory comprises classes implementing interfaces for database operations. This layer interacts with the database using libraries like Dapper.

#### Connections and Repository Implementations:

  **DapperDBConnection.cs:** 
  
    Manages database connections.
    Utilizes MySQL for connecting and executing commands.

  **DataSammenstillingSaksBRepository.cs:** 
  
    Implements complex queries associated with multiple data tables.
    Fetches detailed reports and paginated data overviews and pagination features.

  **GjesteinnmelderRepository.cs:**
    
    Manages database interactions for "gjesteinnmeldger".
    Provides methods for creating and validating guest entries.

  **GeometriRepository.cs:**
    
    Manages database interactions for the geometry.
    Handles fetching and updating geometries from the database.

  **InnmeldingRepository.cs:**
    
    Manages database interactions for "innmelding".
    Implements fetching, updating, and counting reports. Manages enum field operations and "innmelder" updates.

  **VurderingRepository.cs:**
    
    Manages evaluation-related database tasks.
    Features methods for adding and viewing assessments and associated evaluations.

  **TransaksjonsRepository.cs:**
    
    Handles transaction operations for creating and managing composite report entries.
    Manages complex transactions between entities.

---
<br><br>

## Technologies Used:

- ASP.NET Core MVC: For building the web application with a structured MVC pattern.
- C#: Main language for backend logic.
- Dapper ORM: For lightweight and fast database operations.
- MySQL/MariaDB: Database used for storing and querying user and geospatial data.
- Leaflet.js: JavaScript library for interactive maps.
- Kartverket API: For map data and geolocation services.

### Why We Used the Technologies We Used

- ASP.NET MVC: Chosen for its robust framework and built-in support for Model-View-Controller architecture, making it easier to manage different components of the application effectively.
- Dapper ORM: Used for efficient database interaction with MySQL/MariaDB, providing a lightweight and flexible approach to query execution.
- MySQL/MariaDB: Selected as the database system due to its scalability and support.
- Kartverket API & Leaflet.js: Integrated for accurate mapping and visualization of geospatial data, offering users a real-time view of locations on an interactive map.

---

## Collaborators:
- https://github.com/OlsenLene
- https://github.com/Galescape
- https://github.com/TriggeredBanana
- https://github.com/Kristiann96
- https://github.com/Martinstomnas
- https://github.com/emkaas

---

## License

The project is licensed under the MIT License. <br><br>
Read more about the MIT License here: https://choosealicense.com/licenses/mit/
