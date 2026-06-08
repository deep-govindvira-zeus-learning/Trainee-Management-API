# Trainee Management Api
 
## Technology Used
C# And .NET CORE
 
## Features Completed
1. Created a Asp.Net Web Api Project and successfully tested Swagger UI for API Testing.  
2. Created Health API  
3. Created Trainee Model  
4. Created In-Memory Trainee List  
5. Created GET, POST, PUT, DELETE methods for trainee.
6. Created TraineeDTO, TraineeController, TraineeService, TraineeConverter.
7. Added Validations on request.
8. Added AppDbContext and search Api.
 
## How to Run
```
git clone https://github.com/deep-govindvira-zeus-learning/Trainee-Management-API

cd Trainee-Management-API

dotnet run
```

## Challenges Faced
- I faced challenges while creating the api. To overcome challenges i used help of official documentation of .NET Core and C#  
- I faced challenges while addding service layer and updating conroller
- I faced challenges while downloading a package due to aws restrictions. 

## Api Endpoints

### 1. Get All Trainees
Retrieves list of trainees.

* **HTTP Method:** `GET`
* **Path:** `/api/trainees`

#### Query Parameters

| Parameter | Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `search` | string | No | Searching on multiple fileds of Trainee (Default: "") |

#### Example Response (`200 OK`)
```json
[
  {
    "id": "23113761-3309-45ad-82ad-8d532b0877a2",
    "firstName": "Deep",
    "lastName": "Govindvira",
    "email": "deep.govindvira@zeuslearning.com",
    "techStack": "C#, .NET",
    "status": "Active",
    "createdDate": "2026-06-08T10:10:52.2837428Z",
    "updatedDate": "2026-06-08T10:10:52.2837871Z"
  },
  {
    "id": "6245b3e1-decf-46dc-9ba4-4729b2d2563d",
    "firstName": "Yash",
    "lastName": "Gokulgandhi",
    "email": "yash.gokulgandhi@zeuslearning.com",
    "techStack": "C#, .NET",
    "status": "Pending",
    "createdDate": "2026-06-08T10:12:09.2798Z",
    "updatedDate": "2026-06-08T10:12:09.2798003Z"
  }
]
```

---

### 2. Get Trainee By Id

Retrieves trainee by Id.

* **HTTP Method:** `GET`
* **Path:** `/api/trainees/{id}`

#### Example Response (`200 Ok`)
```json
{
    "id": "23113761-3309-45ad-82ad-8d532b0877a2",
    "firstName": "Deep",
    "lastName": "Govindvira",
    "email": "deep.govindvira@zeuslearning.com",
    "techStack": "C#, .NET",
    "status": "Active",
    "createdDate": "2026-06-08T10:10:52.2837428Z",
    "updatedDate": "2026-06-08T10:10:52.2837871Z"
}
```
---

### 3. Create Trainee
Registers a new trainee in the database.

* **HTTP Method:** `POST`
* **Path:** `/api/trainees`

#### Request Body

| Field | Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `firstname` | string | Yes | The trainee's first name |
| `lastName` | string | Yes | The trainee's last name |
| `email` | string | Yes | A unique valid email address |
| `techStack` | string | Yes | The trainee's techstack name |
| `status` | string | Yes | The trainee's status (Must be "Active", "Pending" or "Inactive") |

#### Example Response (`201 Created`)
```json
{
    "id": "23113761-3309-45ad-82ad-8d532b0877a2",
    "firstName": "Deep",
    "lastName": "Govindvira",
    "email": "deep.govindvira@zeuslearning.com",
    "techStack": "C#, .NET",
    "status": "Active",
    "createdDate": "2026-06-08T10:10:52.2837428Z",
    "updatedDate": "2026-06-08T10:10:52.2837871Z"
}
```

---

### 4. Update Trainee

* **HTTP Method:** `PUT`
* **Path:** `/api/trainees/{id}`

#### Request Body

| Field | Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `firstname` | string | Yes | The trainee's first name |
| `lastName` | string | Yes | The trainee's last name |
| `email` | string | Yes | A unique valid email address |
| `techStack` | string | Yes | The trainee's techstack name |
| `status` | string | Yes | The trainee's status (Must be "Active", "Pending" or "Inactive") |

#### Example Response (`200 Ok`)
```json
{
  "id": "23113761-3309-45ad-82ad-8d532b0877a2",
  "firstName": "Yash",
  "lastName": "Gokulgandhi",
  "email": "yash.gokulgandhi@zeuslearning.com",
  "techStack": "Java, Spring Boot",
  "status": "Active",
  "createdDate": "2026-06-08T10:10:52.2837428Z",
  "updatedDate": "2026-06-08T10:19:45.9641777Z"
}
```

---

### 5. Delete Trainee

* **HTTP Method:** `DELETE`
* **Path:** `/api/trainees/{id}`

#### Example Response (`200 Ok`)
