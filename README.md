# Windows Custom Updates Solution

## Overview

This project is light weight patch management tool for Windows based OS.
It helps to monitor and control patch management of OS.
Originally written to help with patch management for Windows Virtual Desktop (WVD) based solutions
but works also very well for other non WVD based solutions or setups.

### Why to use this project?

If you need to do any kind of patch management for Windows based workloads you have many options.
For example System Center Configuration Manager, Windows Update for Business, Windows Update, in case of WVD patch management also injection into base WVD image.

But sometimes, maybe you do not have setup which allows you to use tools highlighted above.
Maybe you have many AD domains and forests, maybe you might be missing some capabilities like reporting, dashboards, telemetry to name few.

This is when you might benefit from this project which is trying to address such needs.

However, our recommendation is always to look first on professional solutions described above
and only if you see some gaps with those professional solutions you might want to check this open source project.

### Features

- Robust NT Service based tool
- Automated updates installation, no manual interventions required
- Flexibility with updates configuration (E.g. you can specify which category of updates should be installed, maybe security patches only)
- Centralized place where telemetry data about monitored hosts is stored
- Powerful query language to analyze telemetry data in very fast way
- Rich Reporting capability based on powerful query language above
- Rich Alerting mechanism based on powerful query language above (E.g. send email when automatic update installation fails)
- Open source tool released under permissive free software MIT license. Anybody can benefit, anybody can contribute, no commercial restrictions.

## Architecture

TODO

## Prerequisities

TODO

## Installation

TODO

## Configuration

TODO

