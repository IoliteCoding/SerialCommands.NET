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

This library is availabele as a nuget package:
- **[IoliteCoding.SerialCommands](https://www.nuget.org/packages/IoliteCoding.SerialCommands/)**: a library with all the core logic.
- **[IoliteCoding.SerialCommands.DependencyInjection](https://www.nuget.org/packages/IoliteCoding.SerialCommands.DependencyInjection/)**: Support for dependency injection.

In addition to the `SerialCommands` library, there is a companion Arduino library that works seamlessly with this .NET library. For Arduino, visit the [IoliteCoding_SerialCommands repository](https://github.com/IoliteCoding/IoliteCoding_SerialCommands).

For detailed documentation and usage examples, please refer to the [Wiki](https://github.com/IoliteCoding/SerialCommands.NET/wiki).
