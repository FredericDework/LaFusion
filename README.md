#  LaFusion API: Seamless PDF Merger

**LaFusion** is a powerful, simple, and reliable RESTful API built on **.NET 8** to effortlessly merge multiple PDF documents into a single file. This API is designed to handle various PDF complexities, including documents containing embedded images or scans.

This project is fully open-source and welcomes contributions!


##  Features

* **Multi-File Merge:** Combines an unlimited number of PDF files into one coherent document.
* **Image Support:** Robust handling of PDF documents containing raster images or scans.
* **RESTful Design:** Simple and clear API endpoint for file submission.
* **.NET 8 Optimized:** Built on the latest, high-performance .NET platform.
* **Swagger Documentation:** Comes with built-in API documentation for easy testing and integration.

##  Technology Stack

* **Framework:** .NET 8 (ASP.NET Core Web API)
* **Language:** C#
* **PDF Engine:** [PdfSharp](http://www.pdfsharp.net/) (Used for reliable PDF manipulation under the MIT License)
* **API Documentation:** Swagger / OpenAPI


## Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed.

### Running Locally

1.  **Clone the Repository:**
    ```bash
    git clone [YOUR_REPOSITORY_URL]
    cd LaFusion.Api
    ```

2.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Run the API:**
    ```bash
    dotnet run
    ```
    The API will typically start on `https://localhost:7113`.

4.  **Access Documentation:**
    Open your browser to the Swagger UI for testing the endpoint:
    `https://localhost:7113/swagger`



##  API Endpoint

The primary endpoint handles the file merging process:

| Method   | Endpoint           | Description                          | Consumes              | Returns                             |
| :---     | :---               | :---                                 | :---                  | :---                                |
| **POST** | `/api/merge/merge` | Merges a list of uploaded PDF files. | `multipart/form-data` | `application/pdf` (The merged file) |

### Request Example

You must send the files as a collection of `IFormFile` under the parameter name `files`.

**Example using a tool like Postman or the Swagger UI:**
* **Body Type:** `form-data`
* **Key:** `files`
* **Value:** Select multiple PDF files.


## Contribution

We welcome contributions! If you have suggestions, bug reports, or want to contribute code, please check out our [CONTRIBUTING.md] (File to be created later) guidelines.

### How to Contribute

1.  Fork the repository.
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.



