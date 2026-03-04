# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-03-03

### Added
- **Clean Architecture Initial Implementation**: Organized the project into Api, Application, Infrastructure, and Domain layers.
- **Authentication**: JWT Bearer token authentication with local configuration.
- **API Versioning**: Support for multiple API versions managed via URL segments.
- **Real-Time Dashboards**: Implemented SignalR Hub for real-time notifications and updates.
- **Documentation**: Integrated Swagger/OpenAPI for easy endpoint exploration.
- **Inventory Management**: Core entities and features for tracking products and stock.
- **Sales Module**: Logic for handling transactions and sales history.
- **Clean Git Workflow**: Professional `.gitignore` and comprehensive `README.md`.

### Changed
- Refactored project structure to follow Clean Architecture patterns.

### Fixed
- Initial boilerplate code and dependency injection registrations.
