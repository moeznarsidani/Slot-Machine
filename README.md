**Overview**
This project provides services for managing slot machine game configurations, player balances, and the actual gameplay, including spinning the machine, calculating wins, and updating player balances. The game configuration includes matrix size (width & height) and win lines. The project uses MongoDB to store player data and game configurations, and implements an automated mechanism to generate win lines based on the matrix size.

**Key Features:**
Game Configuration Management: Allows creating and updating the matrix width, height, and the win lines configuration.
Spin Mechanism: Players can bet and spin the slot machine, where results are generated based on the configuration, and winnings are calculated according to the win lines.
Automatic Win Line Generation: The system automatically generates win lines (horizontal and diagonal) based on the matrix configuration (width and height).
Player Management: Players can have a balance, which is deducted when they make a bet and updated with their winnings after the spin.
Concurrency Control: Ensures that multiple operations on player balances are performed safely with respect to concurrent requests.

**Technologies Used**
.NET 8 for backend API development.
MongoDB for storing game configurations and player data.
Swagger for API documentation and testing.

**Business Logic & Key Features**

**Win Line Calculation:**
The system automatically generates win lines based on the matrix size (width and height). These win lines include:

Horizontal Rows: Across each row.
Diagonals: Two types of diagonal paths:
Diagonal from the top left to the bottom right.
Diagonal from the bottom left to the top right.

**Spin Process:**
When a player spins the slot machine, the following steps are executed:

Validate Player: Ensure the player exists and has enough balance to make the bet.
Generate Result Matrix: Randomly generate a result matrix based on the configuration.
Calculate Wins: For each win line, calculate the winnings based on the consecutive matching symbols.
Update Balance: Deduct the bet amount and add the winnings to the player's balance.
Unit Tests
Unit tests are written to ensure the correct functionality of the services:

SlotMachineServiceTests: Tests the spin logic, win line calculation, and balance update.
GameConfigServiceTests: Tests the configuration update and retrieval methods.
Test Coverage:
Validating the spin process, ensuring that balances are updated correctly.
Testing automatic win line generation based on matrix configuration.
Ensuring the correct handling of invalid player requests (e.g., insufficient balance).

Conclusion
This project implements a simple slot machine game with dynamic configuration management and automatic win line generation based on the matrix size. It is designed with a focus on maintainability and extensibility, using MongoDB for persistent data storage, and unit testing to ensure correctness across different features.
