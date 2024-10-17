# MimoBackend_Oct24

## Overview

MimoBackend_Oct24 is a simple ASP.NET Core server application designed to simulate the functionality of the Mimo app. The server communicates with a SQLite database using Entity Framework Core to manage courses, chapters, lessons, user progress, and achievements. This project serves as a coding challenge, showcasing how to design server systems and handle RESTful API requests.

## Features

- **Courses**: Three courses available: Swift, JavaScript, and C#.
- **Chapters and Lessons**: Each course consists of multiple chapters with lessons in a specific order.
- **User Progress Tracking**: Tracks completion of lessons and time taken to complete.
- **Achievements**: Updates user achievements based on completed lessons and chapters.

## API Endpoints

### Achievements

- **GET** `/api/achievements/{userId}`
  - **Description**: Retrieve achievements for a specific user.
  - **Parameters**:
    - `userId`: The ID of the user whose achievements are being requested.

### Lesson Progress

- **POST** `/api/lessonprogress/completelesson`
  - **Description**: 
    - 1/ Inserts information about a lesson that a user has completed.
    - 2/ Then checks and updates the acheivements for that user. **NB This is currently called from the controller, 
however this could become a regularly run separate service for a scalable app.**
    
  - **Request Body**: 
    ```json
    {
      "userId": int,
      "lessonId": int,
      "startTime": "string", // ISO 8601 format
      "completionTime": "string" // ISO 8601 format
    }
    ```

## Pre-seeded Data

The following data is pre-seeded in the database for testing and demonstration purposes:

### Users
| UserId | UserName |
|--------|----------|
| 1      | John     |
| 2      | Paul     |
| 3      | Jane     |
| 4      | Sarah    |

### Courses
| CourseId | Title       |
|----------|-------------|
| 1        | Swift       |
| 2        | Javascript  |
| 3        | C#          |

### Chapters
| ChapterId | Title              | CourseId |
|-----------|-------------------|----------|
| 1         | Swift Basics      | 1        |
| 2         | Swift Advanced     | 1        |
| 3         | Javascript Basics  | 2        |
| 4         | Javascript Advanced | 2        |
| 5         | C# Basics          | 3        |
| 6         | C# Advanced        | 3        |

### Lessons
| LessonId | Title                          | ChapterId |
|----------|--------------------------------|-----------|
| 1        | Swift Basics Lesson 1          | 1         |
| 2        | Swift Basics Lesson 2          | 1         |
| 3        | Swift Advanced Lesson 1        | 2         |
| 4        | Swift Advanced Lesson 2        | 2         |
| 5        | Javascript Basics Lesson 1     | 3         |
| 6        | Javascript Basics Lesson 2     | 3         |
| 7        | Javascript Advanced Lesson 1    | 4         |
| 8        | Javascript Advanced Lesson 2    | 4         |
| 9        | C# Basics Lesson 1             | 5         |
| 10       | C# Basics Lesson 2             | 5         |
| 11       | C# Advanced Lesson 1           | 6         |
| 12       | C# Advanced Lesson 2           | 6         |

### Achievements
| AchievementId | Title                        | Target |
|----------------|------------------------------|--------|
| 1              | Complete 5 Lessons           | 5      |
| 2              | Complete 25 Lessons          | 25     |
| 3              | Complete 50 Lessons          | 50     |
| 4              | Complete 1 Chapter           | 1      |
| 5              | Complete 5 Chapters          | 5      |
| 6              | Complete Swift Course        | 1      |
| 7              | Complete Javascript Course   | 1      |
| 8              | Complete C# Course          | 1      |

## Getting Started

### Prerequisites

- .NET 6.0 SDK or higher
- SQLite

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/MimoBackend_Oct24.git
   cd MimoBackend_Oct24
