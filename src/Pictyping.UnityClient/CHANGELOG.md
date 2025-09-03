# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-09-03

### Added
- Initial release of Pictyping Unity Client
- Unity WebRequest-based API client for Pictyping API
- Support for authentication (login, Google OAuth callback, exchange code)
- Rating update functionality
- Ranking retrieval functionality
- JWT token authentication support
- Unity Package Manager (UPM) support
- Comprehensive Unity integration documentation
- Basic usage examples in `Samples~/BasicUsage/`
- Advanced usage examples with UI integration in `Samples~/AdvancedUsage/`
- Network management with offline support and retry logic
- Automatic API client generation from OpenAPI specification
- Cross-platform sync scripts for Unity project integration

### Dependencies
- Unity 2020.3 LTS or higher
- Newtonsoft.Json for Unity (com.unity.nuget.newtonsoft-json 3.0.2+)
- .NET Standard 2.1 compatible

### API Coverage
- AuthApi: Cross-domain login, Google login, exchange code operations
- PictypingAPIApi: Core game functionality including rating updates
- RankingApi: Ranking data retrieval

### Documentation
- Complete Unity integration guide (`UNITY_INTEGRATION.md`)
- Sample code with error handling best practices
- Network management patterns for robust API communication
- Offline mode and request queuing capabilities