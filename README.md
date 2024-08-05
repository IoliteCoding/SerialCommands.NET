# SerialCommands Library for .NET

## Overview

The `SerialCommands` library provides a robust framework for managing and communicating serial commands between devices. Designed to facilitate reliable data transmission, this library enables developers to define, send, and receive structured commands over serial interfaces, ensuring efficient and error-free communication.

## Purpose

The primary goal of the `SerialCommands` library is to simplify the process of implementing serial communication protocols. It allows for:

- **Structured Command Format**: Define commands with clear start and stop markers, optional data fields, and flexible address ranges.
- **Data Integrity**: Ensured by the address length byte, which provides a clear structure for the length of the command, including address and data.
- **Flexible Addressing**: Utilize a base-200 system to support a wide range of addresses, accommodating both simple and complex communication needs.
- **Ease of Use**: Implement and manage serial communication effortlessly with a user-friendly API.

## Key Features

- **Start and Stop Bytes**: Delimit commands clearly to avoid misinterpretation.
- **Optional Data**: Include data in commands when needed, with the flexibility to send commands without data.
- **Extended Addressing**: Support for single and multiple byte addresses using a base-200 system for extended range.
- **Address Length**: Easily manage varying address lengths to accommodate different command structures and requirements.

## Additional Resources

In addition to the `SerialCommands` library, there is a companion Arduino that works seamlessly with this .NET library. For Arduino, visit the [IoliteCoding_SerialCommands repository](https://github.com/IoliteCoding/IoliteCoding_SerialCommands).
