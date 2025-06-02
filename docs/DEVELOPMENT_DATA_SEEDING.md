# Development Data Seeding

This document describes the automatic data seeding feature for the development environment.

## Overview

When running the application in development mode, dummy data can be automatically inserted into the database on startup. This feature helps developers quickly set up a working environment with test data.

## Configuration

The data seeding is controlled by the `DataSeeding:SeedDataOnStartup` configuration in `appsettings.Development.json`:

```json
{
  "DataSeeding": {
    "SeedDataOnStartup": true
  }
}
```

## Seeded Data

When enabled, the following test data is created:

### Users
- **Admin User** (admin@example.com) - Admin privileges, Rating: 1500
- **Player One** (player1@example.com) - Regular user, Rating: 1200
- **Player Two** (player2@example.com) - Regular user, Rating: 1350
- **Guest Player** (guest@example.com) - Guest user, Rating: 1000

All test users have the password: `password123`

### Typing Matches
Several completed typing matches between the test users with various scores and accuracy levels.

### OAuth Identities
Google OAuth identities for the admin and player1 users.

## Usage

1. Ensure you're running in Development environment
2. Set `SeedDataOnStartup` to `true` in appsettings.Development.json
3. Start the application
4. The seeding will run automatically on startup if the database is empty

## Notes

- Seeding only occurs if no users exist in the database
- The seeding process is logged for debugging purposes
- This feature is only available in Development environment
- Passwords are hashed using SHA256 (for development only)