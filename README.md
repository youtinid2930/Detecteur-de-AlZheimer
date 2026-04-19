# Detecteur de Alzheimer

A web application for Alzheimer's disease detection from brain scan images. Built with ASP.NET Core (C#) as the web framework, it integrates a pre-trained Python machine learning model to analyze uploaded medical images and return a predicted class with confidence score.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Web Framework | ASP.NET Core (C#), MVC pattern |
| ML Model | Python, Keras (`.keras` model file) |
| Image Processing | Uploaded via web form, passed to Python subprocess |
| Database | Entity Framework Core + SQL Server (migrations) |
| Frontend | Razor Views, HTML, CSS, JavaScript |
| Languages | Python 97.9%, HTML 1.3%, C# 0.5% |

---

## How It Works

1. User uploads a brain scan image through the web interface
2. The `DetectionController` saves the image to `wwwroot/uploads/`
3. It spawns a Python subprocess running `modele/app.py`, passing:
   - the `.keras` model path
   - a `classes.json` label map
   - the uploaded image path
4. Python runs inference and returns: `Classe: <label>  Confiance: <percentage>`
5. The result is parsed and returned as JSON to the frontend
6. The uploaded image is deleted after prediction

---

## Controllers

| Controller | Responsibility |
|---|---|
| `HomeController` | Landing page |
| `DetectionController` | Image upload, Python subprocess call, result parsing |
| `SvmController` | Alternative SVM model integration |
| `PatientController` | Patient management |
| `DoctorController` | Doctor management |
| `AccountController` | User authentication |

---

## Project Structure

```
├── Controllers/
│   ├── DetectionController.cs   # Core detection logic (image upload → Python → result)
│   ├── SvmController.cs         # SVM model integration
│   ├── PatientController.cs     # Patient management
│   ├── DoctorController.cs      # Doctor management
│   ├── AccountController.cs     # Auth
│   └── HomeController.cs        # Landing
├── Models/                      # C# data models
├── Views/                       # Razor view templates
├── Services/                    # Business logic services
├── Data/                        # EF Core DbContext
├── Migrations/                  # Database migrations
├── modele/
│   ├── app.py                   # Python inference script (teammate's work)
│   ├── mymodel.keras            # Pre-trained Keras model (teammate's work)
│   └── classes.json             # Label mapping
├── wwwroot/                     # Static files, uploaded images (temp)
├── Program.cs                   # App entry point and DI configuration
├── requirements.txt             # Python dependencies
└── Alzheimer Detection.csproj
```

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Python 3.10
- SQL Server (or update connection string for SQLite)

### Install Python dependencies

```bash
pip install -r requirements.txt
```

### Setup & Run

```bash
git clone https://github.com/youtinid2930/Detecteur-de-AlZheimer.git
cd Detecteur-de-AlZheimer

# Update appsettings.json with your DB connection string
# Update Python path in DetectionController.cs if needed

dotnet ef database update
dotnet run
```

Then open `https://localhost:5001` in your browser.

---

## My Contribution

- Set up the ASP.NET Core project structure and Git repository
- Wired the `DetectionController` to call the Python model via subprocess and parse its output
- Managed pull requests and resolved merge conflicts during team integration
- Integrated all team contributions into the final working application

---

## Contributors

- **[youtinid2930](https://github.com/youtinid2930)** — Web integration, project setup, merge management
- **[YASSINE-TLILI](https://github.com/YASSINE-TLILI)** — ML model (Python/Keras), inference script
- **[MohamedAgdid](https://github.com/MohamedAgdid)** — Additional controllers and views

