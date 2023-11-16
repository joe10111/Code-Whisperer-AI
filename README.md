# Code Whisperer AI
## Introduction
**Code Whisperer AI** is a capstone project developed for Turing's Launch program, showcasing the cumulative learning from mods 0 to 5. This individual project spaned two weeks and represents a blend of skills and knowledge acquired through the rigorous program.

**Code Whisperer AI** is your personal guide to skill refinement and code perfection. Tailored for both beginners and seasoned coding professionals, our application leverages the power of GPT-4 API to provide insightful analysis and actionable feedback for your code. It's designed to operate quietly yet deliver impactful enhancements, ensuring your code's quality and performance stand out.

## Features
AI-Powered Analysis: Utilizing the GPT-4 API for advanced code analysis.
User-Friendly Interface: Simple and intuitive design for easy interaction.
Comprehensive Feedback: Detailed insights on code structure, performance, and style.
Skill Development: Ideal for learners and professionals aiming to refine their coding skills.
## Installation
To set up Code Whisperer AI, follow these steps:

1. **Clone the Repository**:
```
git clone https://github.com/your-repository/code-whisperer-ai.git
```
2. **Navigate to the Project Directory**:
```
cd code-whisperer-ai
```
3. **Install Dependencies**:

Ensure you have the following key dependencies installed:

- **Entity Framework Core**: For object-relational mapping.
- **Npgsql**: PostgreSQL provider for Entity Framework Core.
- **Serilog**: For logging in .NET applications.
- **Microsoft ASP.NET Core Identity**: For user authentication and authorization.

Install them using the NuGet package manager. For example:
```
dotnet add package Microsoft.EntityFrameworkCore --version 7.0.13
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 7.0.11
dotnet add package Serilog --version 3.0.1
dotnet add package Serilog.Sinks.Console --version 4.1.0
dotnet add package Serilog.Sinks.File --version 5.0.0
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 6.0.23
```

4. **Start the Application**:
```
dotnet run
```

## Usage
After launching Code Whisperer AI, follow these steps to analyze your code:
- Navigate to the Analyze page.
- Enter your code snippet into the provided input area.
- Click the "Analyze" button to submit your code for review.
- View the AI-generated feedback and suggestions in the results area.

## Contributing
Contributions to Code Whisperer AI are welcome! If you have suggestions or improvements, feel free to fork this repository and submit a pull request. For major changes, please open an issue first to discuss what you would like to change.

## Acknowledgments
- Turing's Launch Program: For providing the educational platform and resources that made this project possible.
- OpenAI's GPT-4 API: Powers the core functionality of Code Whisperer AI, offering advanced AI-powered code analysis.
- Ace Editor: An embeddable code editor for the web, used in Code Whisperer AI for providing a user-friendly interface for code input with features like syntax highlighting and line numbering.
- Bootstrap: For its comprehensive front-end framework, which enhanced the UI/UX design of the application.
- jQuery: Used for DOM manipulation and to facilitate some of the interactive features in the application.
