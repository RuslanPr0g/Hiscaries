# 📚 Hiscaries Library of Stories

---

[C4 Diagram of the system](https://drive.google.com/file/d/1YVpVJS43djNFkMbAwFCGF1CzXhPL_sf-/view?usp=sharing)

---

## 🚀 Local Development Setup 🚀

### Prerequisites

You will need

- Docker Engine
- DotNet CLI and SDK

to run the application locally.

### Backend

To set up the backend environment, simply run:

```powershell
SetUp-LocalEnv.ps1
```

or bash if you're on linux

```bash
chmod +x setup-localenv.sh

setup-localenv.sh
```

This will configure the backend using .NET Aspire.

```powershell
Add-Migration.ps1
```

This will add a new migration to the chosen service.

To run the backend application, please set Hiscary.AppHost as your startup project and hit run!

#### Domain-Driven Design (DDD) with StackNucleus

This application leverages **StackNucleus**, a package that helps in modeling Domain-Driven Design (DDD) in .NET. StackNucleus is integrated into the backend to structure the domain logic and ensure clean, maintainable architectures. It simplifies the modeling of aggregates, entities, and value objects in a way that aligns with DDD principles.
See more here: https://github.com/RuslanPr0g/StackNucleus

#### Helpful!

List your user secrets:

```bash
chmod +x list-usersecrets.sh
list-usersecrets.sh
```

Build and run on linux (given you are within the /server directory):

```bash
dotnet restore

dotnet build

newgrp docker # optional

dotnet run --project ./Hiscary.AppHost/Hiscary.AppHost.csproj
```

### Frontend

Navigate to the frontend (client) directory, then run:

```bash
npm ci
npm run start
```

For full frontend setup, team conventions, and all commands, see
[`client/README.md`](client/README.md).

---

🎉 **Thank you for visiting the repository!**

Feel free to contribute or raise any issues to improve the application!
