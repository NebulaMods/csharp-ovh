# OVH API Client for .NET

[![NuGet version](https://badge.fury.io/nu/ovh-api.svg)](https://badge.fury.io/nu/ovh-api)
[![Build Status](https://github.com/NebulaMods/ovh-api/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/NebulaMods/ovh-api/actions)

This is a .NET client library for interacting with the OVH API, enabling developers to manage OVH resources programmatically using C# and .NET.

## Installation

You can install the package via NuGet:

```shell
dotnet add package ovh-api
```

Or by adding it to your project file:

```xml
<PackageReference Include="ovh-api" Version="x.x.x" />
```

## Usage

Here's a basic example of how to use the OVH API client in your .NET project:

```csharp
using Ovh.Api;

var client = new OvhClient(
    applicationKey: "your_application_key",
    applicationSecret: "your_application_secret",
    consumerKey: "your_consumer_key",
    endpoint: "https://eu.api.ovh.com/1.0/"
);

// Example: Retrieve a list of domains
var domains = await client.GetAsync<string[]>("/domain");

// Example: Get account information
var accountInfo = await client.GetAsync<AccountInformation>("/me");
```

## Features

- Simple and intuitive API for interacting with OVH services.
- Supports multiple OVH endpoints (e.g., Europe, Canada).
- Handles authentication and request signing automatically.

## Supported Endpoints

- **Europe V1:** `https://eu.api.ovh.com/1.0/`
- **Canada V1:** `https://ca.api.ovh.com/1.0/`
- **US V1:** `https://api.us.ovhcloud.com/1.0/`
- **Europe V2:** `https://eu.api.ovh.com/2.0/`
- **Canada V2:** `https://ca.api.ovh.com/2.0/`
- **US V2:** `https://api.us.ovhcloud.com/2.0/`

## Documentation

For full API documentation, please visit the official OVH API documentation: [https://api.ovh.com/](https://api.ovh.com/)

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

1. Fork it (https://github.com/NebulaMods/ovh-api/fork)
2. Create your feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Create a new Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

If you have any questions or need support, feel free to open an issue on GitHub or contact me directly.

---

This README includes installation instructions, usage examples, features, supported endpoints, and contribution guidelines. Adjust any sections as needed based on your project's specifics.