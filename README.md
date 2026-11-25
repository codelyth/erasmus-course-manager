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

![Status](https://img.shields.io/badge/Status-Early_Development-155EEF?style=flat-square)
![Academic Project](https://img.shields.io/badge/University_of_Bielsko--Biała-Programming_3_%26_AI_Tools-163B65?style=flat-square)

</div>

---

## 🌍 About the Project

Erasmus Course Manager is an individual semester project developed for the **Programming 3** and **AI Tools** courses at the **University of Bielsko-Biała**.

The application will help students register for courses, academicians view their assigned courses and students, and administrators manage accounts, courses, and registrations.

The project combines C# desktop development with a separate Python computer vision service. Development starts with the course management application; AI authentication will follow.

> 🚧 **Current stage:** Project setup and domain modeling. Course management, authentication, persistence, and AI features are planned and are not yet implemented.

## ✅ Current Progress

- [x] Create the .NET 10 Windows Forms project.
- [x] Verify that the initial application runs.
- [x] Create the Models, Services, Interfaces, Repositories, and Forms folders.
- [x] Implement the abstract `User` model with basic constructor validation and encapsulated properties.
- [ ] Implement `Student`, `Academician`, `Administrator`, `Course`, and `Registration`.
- [ ] Implement business rules, LINQ queries, and C# events.
- [ ] Add TXT file persistence.
- [ ] Add password authentication and authorization.
- [ ] Build role-based application screens.
- [ ] Develop and integrate the Python face authentication service.
- [ ] Complete evaluation, documentation, and demo.

## 👥 Planned User Roles

| Role | Capabilities |
| --- | --- |
| 🛡️ Administrator | Manage user accounts and courses, assign academicians, manage registrations, and view all enrollments and capacities. |
| 👩‍🏫 Academician | View assigned courses, list enrolled students, search assigned courses, and view capacities. |
| 🎒 Student | Browse and search courses, enroll when capacity is available, cancel personal registrations, and view enrolled courses. |

All roles will support **email/password login** and optional **face recognition login**. Users without a face enrollment will be able to use their passwords.

## 🧰 Technology Stack

| Area | Technologies | Status |
| --- | --- | --- |
| Desktop application | C#, .NET 10, Windows Forms | Project initialized |
| Application logic | OOP, LINQ, C# Events | Domain modeling started |
| Persistence | TXT files | Planned |
| Computer vision | Python, OpenCV, MediaPipe | Planned |
| Face recognition | ArcFace-based embeddings | Planned |
| Model development | PyTorch training / fine-tuning | Planned |
| Presentation attack detection | Active liveness and passive anti-spoofing | Planned |
| C#–Python communication | Local FastAPI service | Planned |
| Model interchange | ONNX, if needed | Optional |
| Development tools | Visual Studio 2026, PyCharm, Git, GitHub | Selected tools |

## 🧱 Architecture

The initial C# implementation uses one Windows Forms project with separate folders for responsibilities:

| Folder | Responsibility |
| --- | --- |
| `Models/` | Domain entities and their state. |
| `Services/` | Business rules, course registration, and authentication workflows. |
| `Interfaces/` | Contracts for interchangeable components. |
| `Repositories/` | Loading and saving application data. |
| `Forms/` | Windows Forms screens and UI interactions. |

The planned domain model consists of an abstract `User` class with `Student`, `Academician`, and `Administrator` subclasses, plus `Course` and `Registration` entities.

A registration links one student to one course. Each course may have **zero or one academician**, while an academician may teach multiple courses.

The Python module will run separately and communicate with C# through a local API. C# will validate the returned user identity and account status before granting access.

> 📁 The Python module and supporting data, model, test, and documentation directories will be added in later stages. Empty folders may not appear in Git.

## 📋 Planned Business Rules

- A student cannot register for the same course twice.
- A full course cannot accept additional registrations.
- Course capacity cannot be reduced below the current enrollment count.
- Related registrations must be checked before deleting a student or course.
- Users may perform only the operations allowed for their role.
- Passwords must be stored as hashes, never as plaintext.
- An operation must not report success if its required file save fails.
- Face authentication must resolve to an existing, active user account.

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

**Current expected behavior:** A blank `Form1` window opens. The `User` model exists but is not connected to the UI yet.

Python dependencies and setup instructions will be documented when the AI module is introduced.

## 🗺️ Development Roadmap

| Phase | Focus |
| --- | --- |
| 01 | 🔧 Project setup and domain models — **in progress** |
| 02 | 🧠 Business logic and OOP |
| 03 | 📂 LINQ, events, and file persistence |
| 04 | 🔑 Password authentication and authorization |
| 05 | 🖥️ Windows Forms UI |
| 06 | 🐍 Python and computer vision fundamentals |
| 07 | 📷 OpenCV and MediaPipe |
| 08 | 🧬 ArcFace and PyTorch model training / fine-tuning |
| 09 | 🛡️ Liveness detection and anti-spoofing |
| 10 | 🔗 Python–C# integration |
| 11 | 🧪 Testing, documentation, and demo |

## 📐 UML Design

The design references include use case, class, component, authentication activity, and face recognition login sequence diagrams. A course registration sequence diagram is also planned for the documentation set.

Diagram files are added under `docs/uml/` path.

## 👩‍💻 Author

**Yaprak Cihantimur** · [@codelyth](https://github.com/codelyth)

Developed as an individual academic project for **Programming 3 & AI Tools** at the **University of Bielsko-Biała**.

---

<div align="center">

💙 Built step by step, with a focus on understanding the code.

</div>
