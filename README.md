<div align="center">

# 🎓 Erasmus Course Manager

### Course Registration & Management System
#### with AI-Powered Face Recognition & Anti-Spoofing

A Windows desktop application for Erasmus students, academicians, and administrators.

![C#](https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows Forms](https://img.shields.io/badge/UI-Windows_Forms-0078D4?style=for-the-badge)
![Python](https://img.shields.io/badge/Python-Planned-3776AB?style=for-the-badge&logo=python&logoColor=white)
![PyTorch](https://img.shields.io/badge/PyTorch-Planned-EE4C2C?style=for-the-badge&logo=pytorch&logoColor=white)

![Status](https://img.shields.io/badge/Status-In_Development-155EEF?style=flat-square)
![Academic Project](https://img.shields.io/badge/University_of_Bielsko--Biała-Programming_3_%26_AI_Tools-163B65?style=flat-square)

</div>

---

## 🌍 About the Project

Erasmus Course Manager is an individual semester project developed for the **Programming 3** and **AI Tools** courses at the **University of Bielsko-Biała**.

The application is being developed to help students register for courses, academicians view their assigned courses and students, and administrators manage accounts, courses, and registrations. Account management and course browsing are available; enrollment and course management screens are still in progress.

The project combines C# desktop development with a separate Python computer vision service. The C# application is under active development; AI authentication will follow.

> 🚧 **Current Stage:** Core models, local persistence, password authentication, authorization, initial administrator setup, and user management screens are implemented. Course browsing is available for all roles. Course management, enrollment screens, and the Python AI module are next.

## ✅ Current Progress

### Completed

- [x] Set up the .NET 10 Windows Forms project and Git repository.
- [x] Implement `User`, `Student`, `Academician`, `Administrator`, `Course`, and `Registration` models.
- [x] Add constructor validation and encapsulated model properties.
- [x] Implement user, course, and registration services with LINQ queries.
- [x] Add registration-created and registration-cancelled C# events.
- [x] Save and load application data as JSON text in a TXT file.
- [x] Implement PBKDF2 password hashing, password login, and logout.
- [x] Add shared session management and role checks in manager methods.
- [x] Add first-run administrator account creation.
- [x] Build the login screen and a shared dashboard adapted to the user's role.
- [x] Show searchable course lists and available capacity.
- [x] Build administrator user management: create, edit, activate/deactivate, and delete accounts.
- [x] Add user search, role filtering, and administrator password reset with confirmation.
- [x] Centralize UI colors, primary button styling, and shared form layout helpers.

### Remaining

- [ ] Build course creation, editing, deletion, and academician assignment screens.
- [ ] Build student enrollment, cancellation, and personal registration screens.
- [ ] Display enrolled students for an academician's assigned courses.
- [ ] Build administrator registration management and an all-registrations view.
- [ ] Complete end-to-end role, capacity, persistence, and UI checks.
- [ ] Develop, train/fine-tune, evaluate, and integrate the Python AI module.
- [ ] Align UML diagrams with the final implementation and complete the project report and demo.

## 👥 User Roles

| Role | Available in the UI | Remaining UI Work |
| --- | --- | --- |
| 🛡️ Administrator | Password login/logout, user management, password reset, course search, and capacity display. | Course management, academician assignment, and registration management. |
| 👩‍🏫 Academician | Password login/logout, assigned-course search, and capacity display. | Student lists for assigned courses. |
| 🎒 Student | Password login/logout, course search, and capacity display. | Enrollment, cancellation, and personal registration views. |

All three roles currently use **email/password login**. Optional **face recognition login** is planned for the AI phase; password login will remain available.

Course and registration operations already exist in the service layer. Their remaining UI screens will call those services rather than duplicate business rules.

## 🧰 Technology Stack

| Area | Technologies | Status |
| --- | --- | --- |
| Desktop application | C#, .NET 10, Windows Forms | Login, dashboard, and user management available |
| Application logic | OOP, LINQ, C# Events | Core services implemented |
| Persistence | JSON text in a TXT file, System.Text.Json | Implemented |
| Authentication | PBKDF2-SHA256, random salts, shared session | Implemented |
| Computer vision | Python, OpenCV, MediaPipe | Planned |
| Face recognition | ArcFace-based embeddings | Planned |
| Model development | PyTorch training / fine-tuning | Planned |
| Presentation attack detection | Active liveness and passive anti-spoofing | Planned |
| C#-Python communication | Local FastAPI service | Planned |
| Model interchange | ONNX, if needed | Optional |
| Development tools | Visual Studio 2026, PyCharm, Git, GitHub | Selected tools |

## 🧱 Architecture

The C# application uses one Windows Forms project with separate responsibilities:

| Folder | Responsibility |
| --- | --- |
| `Models/` | Domain entities, the `AppData` container, and registration event arguments. |
| `Services/` | Business rules, authentication, authorization, shared state, and service initialization. |
| `Configuration/` | Central application data paths. |
| `Repositories/` | Loading and saving application data. |
| `Forms/` | Login, initial setup, dashboard, and user management screens. |
| `Styling/` | Shared `AppTheme` colors/styles and `FormLayout` helpers. |
| `Interfaces/` | Reserved for component contracts, including future AI integration. |

The domain model uses an abstract `User` class with `Student`, `Academician`, and `Administrator` subclasses. A `Registration` links one student to one `Course`. Each course may have **zero or one academician**, while an academician may teach multiple courses.

| Component | Responsibility |
| --- | --- |
| `AppServices` | Creates one shared set of services for the application. |
| `AppDataStore` | Holds in-memory data and commits changes only after saving succeeds. |
| `FileRepository` | Serializes and deserializes data, writing through a temporary file before replacing the saved file. |
| `UserManager` | Validates and manages accounts, initial administrator setup, and password resets. |
| `CourseManager` | Manages courses, academician assignments, and course validation. |
| `RegistrationManager` | Handles enrollment/cancellation, capacity queries, and registration events. |
| `AuthService` | Verifies credentials, opens a session, and logs out. |
| `UserSession` | Resolves the current active user and applies role/ownership checks. |
| `PasswordHasher` | Creates salted PBKDF2-SHA256 hashes and verifies passwords. |

Forms call the managers; managers apply business rules and use the shared store. Persistence and UI styling each have a dedicated implementation.

### Local Data Storage

Users, courses, and registrations are saved together in:

```text
%LocalAppData%\ErasmusCourseManager\Data\application-data.txt
```

The file contains readable JSON text. Saving one data snapshot keeps the related lists together. Data is stored outside the repository and build output folders, so rebuilding the project does not remove saved accounts.

Each Windows user has separate local data. The current version does not synchronize records across computers or provide concurrent multi-user access through a server.

### Future AI Integration

The Python module will run separately and communicate with C# through a local FastAPI service. C# will validate the returned user identity and account status before granting access. AI interfaces, the Python client, and model assets have not been implemented yet.

## 📋 Business Rules

The current service layer enforces:

- Unique email addresses across accounts and unique student numbers.
- No duplicate registration for the same student and course.
- No additional enrollment when a course is full.
- No reduction of course capacity below its current enrollment count.
- Cancellation of related registrations before deleting a student or course.
- Assignment only to an existing, active academician.
- Removal of course assignments before deleting or deactivating an academician.
- Protection of the last active administrator from deletion or deactivation.
- Administrator-only account and course management.
- Student access only to their own registrations.
- Academician access only to their assigned courses and those courses' registration lists.
- Password hashing rather than plaintext password storage.
- In-memory changes committed only after the required save succeeds.
- Registration events raised after persistence succeeds.

Initial administrator creation is permitted only when no administrator account exists. Creating additional administrators requires an authenticated administrator. Existing user roles cannot be changed through the user editor.

Face identity and liveness checks will be implemented in the AI phase.

## 🔐 Planned Face Authentication

The proposed pipeline combines:

1. **OpenCV:** Capture camera frames.
2. **MediaPipe:** Detect the face and extract facial landmarks.
3. **Active liveness:** Request randomized blink or head movement challenges.
4. **Passive anti-spoofing:** Evaluate signs of presentation attacks, such as printed photos or screen replays.
5. **ArcFace:** Compare facial embeddings with enrolled templates.
6. **C# account validation:** Check the matched user and open the appropriate dashboard.

The AI phase will include a real **PyTorch training or fine-tuning process**, with the target model and dataset selected during that phase.

> 🧪 Anti-spoofing will be evaluated using genuine and spoof attempts. It is an experimental academic feature and is not assumed to provide perfect protection.

## 🚀 Run the Current Application

### Requirements

- Windows
- Visual Studio 2026 with the **.NET desktop development** workload
- .NET 10 SDK
- Git

### Steps

```bash
git clone https://github.com/codelyth/erasmus-course-manager.git
cd erasmus-course-manager
```

Open `ErasmusCourseManager.slnx` in Visual Studio, set `ErasmusCourseManager.WinForms` as the startup project if necessary, and press **F5**.

### First Run

If the local data has no administrator account, an **Initial Setup** screen opens. Enter a full name, an unused email address, and a password of at least eight characters, then confirm the password. The account is saved before setup completes.

The application then opens **Sign In**. Subsequent launches skip setup while an administrator account exists. No default administrator password is bundled with the application.

### Current Workflow

1. Sign in with an active account.
2. View and search the course catalogue. Academicians see only their assigned courses.
3. As an administrator, open **Manage Users** to create accounts, edit details and status, delete eligible accounts, or reset passwords.
4. Use **Sign Out** to return to the login screen. Closing the dashboard exits the application.

Email addresses currently act as login identifiers; the app does not send verification or password recovery emails. Account creation is managed by administrators after initial setup.

User management changes and password resets persist across application restarts. Student enrollment and course editing screens are still under development.

Python dependencies and setup instructions will be documented when the AI module is introduced.

## 🗺️ Development Roadmap

| Phase | Focus | Status |
| --- | --- | --- |
| 01 | 🔧 Project Setup and Domain Models | Completed |
| 02 | 🧠 Business Logic and OOP | Core services implemented |
| 03 | 📂 LINQ, Events, and File Persistence | Implemented |
| 04 | 🔑 Password Authentication and Authorization | Implemented |
| 05 | 🖥️ Windows Forms UI | In progress: course and registration screens next |
| 06 | 🐍 Python and Computer Vision Fundamentals | Planned |
| 07 | 📷 OpenCV and MediaPipe | Planned |
| 08 | 🧬 ArcFace and PyTorch Model Training / Fine-Tuning | Planned |
| 09 | 🛡️ Liveness Detection and Anti-Spoofing | Planned |
| 10 | 🔗 Python-C# Integration | Planned |
| 11 | 🧪 Testing, Documentation, and Demo | Manual checks ongoing; final evaluation pending |

## 🧪 Validation So Far

Manual development checks have covered:

- Model creation and inheritance.
- TXT save/load and persistence across application restarts.
- Password hashing, incorrect-password rejection, and login/logout.
- Session checks and selected unauthorized-access scenarios.
- Registration creation/cancellation events, duplicate enrollment rejection, and restored capacity after cancellation.
- Initial administrator setup and skipping setup on later launches.
- Login, course search, and administrator user management workflows.

Full role-by-role acceptance testing, full-capacity enrollment scenarios, save-failure checks, and AI evaluation remain to be completed. The current checks are manual and are not an automated test suite.

## 📐 UML Design

The design references include use case, class, component, authentication activity, and face recognition login sequence diagrams. A course registration sequence diagram is also planned for the documentation set.

Diagram files are stored under `docs/uml/`. The implementation has added shared state, session, hashing, and styling components and uses one data snapshot file. The diagrams will be aligned with these changes before final submission.

## 👩‍💻 Developer

**Yaprak Cihantimur** · [@codelyth](https://github.com/codelyth)

Developed as an individual academic project for **Programming 3 & AI Tools** at the **University of Bielsko-Biała**.
