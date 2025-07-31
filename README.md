# PowerMeasure Web Application

## Overview

PowerMeasure is a full-stack web application built with **.NET** (backend) and **Angular** (frontend) designed to monitor and manage energy consumption on a per-user and per-device basis. It integrates with real-world IoT devices to collect precise energy usage data, provides detailed consumption statistics and visualizations, manages user authentication and billing, and supports payments through the s PayHost platform.

This project is developed as a college assignment to showcase practical skills in web development, IoT communication, real-time data processing, and payment integration.

---

## Features

### User Authentication and Management
- Secure user registration and login.
- Role-based user management.
- Profile management with personalized settings.

### Energy Consumption Monitoring
- Connects to **NETIO PowerCable REST 101x** devices to measure individual device power consumption.
- Uses the **MQTT-flex** protocol to communicate with devices in real-time.
- Stores detailed consumption data per user and device.

### Data Simulation Service
- Background service simulates measurement devices for each household.
- Generates realistic energy consumption data for testing and demonstration.

### Consumption Statistics and Visualizations
- Interactive dashboards showing consumption trends.
- Per-user and per-device energy usage graphs.
- Historical data analysis for better energy management.

### Billing and Payments
- Automatic calculation of monthly energy bills based on consumption data.
- Payment integration with **s PayHost** for secure and seamless transactions.

---

## Architecture

- **Backend:**  
  - ASP.NET Core Web API for business logic, device communication, data processing, and payment handling.
  - Background hosted service simulating device measurements per household.
  - Database storing user info, device data, consumption logs, and billing records.

- **Frontend:**  
  - Angular application for user interface.
  - Responsive and user-friendly dashboards and management pages.
  - Authentication flows and payment UI integrated with backend APIs.

- **Device Communication:**  
  - Uses MQTT-flex protocol to communicate with NETIO PowerCable REST 101x device which is plugged in into some energy consumer.
  - REST API calls to retrieve and control device status.

---

