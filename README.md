# CNAB Processor

CNAB file processing system developed in .NET 9 with PostgreSQL.

> This project was developed to solve the challenge described in [CHALLENGE.md](./CHALLENGE.md).

## 📋 Features

- ✅ CNAB file upload (.txt)
- ✅ Automatic CNAB format parser
- ✅ PostgreSQL database storage
- ✅ Modern and responsive web interface
- ✅ Complete REST API
- ✅ Automatic store balance calculation
- ✅ Docker Compose for development
- ✅ Automated tests
- ✅ API documentation

## 🚀 Technologies Used

- **.NET 9** - Main framework
- **ASP.NET Core** - Web API and MVC
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **Docker** - Containerization
- **xUnit** - Unit tests
- **Swagger** - API documentation

## 🏗️ Architecture

The project follows **Clean Architecture** principles with:

- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Dependency Injection** - Loose coupling
- **Interface Segregation** - Clean contracts
- **Single Responsibility** - Focused classes

### Data Flow:
1. **Controllers** → **Services** → **Repositories** → **Database**
2. **Unit of Work** manages transactions atomically
3. **Repositories** handle all database operations
4. **Services** contain business logic

## 📁 Project Structure

```
CnabProcessor/
├── Controllers/          # API and MVC Controllers
│   ├── CnabController.cs    # CNAB file upload
│   ├── StoresController.cs # Store operations
│   └── HomeController.cs   # Web interface
├── Models/             # Data models
│   ├── Entity/         # Database entities
│   │   ├── Transaction.cs
│   │   └── Store.cs
│   └── Response/       # API response models
├── Repositories/       # Data access layer
│   ├── Data/          # Entity Framework Context
│   ├── Interfaces/    # Repository interfaces
│   └── Migrations/    # Database migrations
├── Services/          # Business services
├── Helpers/           # Utility classes
│   └── TransactionTypeHelper.cs
├── Views/            # MVC Views
├── wwwroot/          # Static files (CSS, JS)
└── Tests/            # Unit tests
```

## 🛠️ Setup and Execution

### Prerequisites

- .NET 9 SDK
- Docker and Docker Compose
- PostgreSQL (if running locally)

### Execution with Docker Compose (Recommended)

1. Clone the repository:
```bash
git clone <repository-url>
cd test-cnab
```

2. Run with Docker Compose:
```bash
docker-compose up --build
```

3. Access the application:
- Web Interface: http://localhost:5000
- Swagger API: http://localhost:5000/swagger (Development only)


### Local Execution

1. Configure PostgreSQL database:
```bash
# Create the database
createdb cnab_processor
```

2. Configure the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cnab_processor;Username=postgres;Password=postgres"
  }
}
```

3. Run migrations:
```bash
cd CnabProcessor
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

## 📊 API Endpoints

### CNAB File Processing

#### POST /api/cnab/upload
Upload and process CNAB file.

**Request:**
- Content-Type: multipart/form-data
- Body: .txt file

**Response:**
```json
{
  "success": true,
  "message": "File processed successfully. 3 stores processed, 15 transactions imported.",
  "processedStores": 3,
  "processedTransactions": 15
}
```

### Store Operations

#### GET /api/stores
List all stores with their balances.

**Response:**
```json
[
  {
    "id": 1,
    "owner": "JOÃO MACEDO",
    "name": "BAR DO JOÃO",
    "balance": 1250.50,
    "transactionCount": 5
  }
]
```

#### GET /api/stores/{storeId}/transactions
Get all transactions for a specific store.

**Response:**
```json
[
  {
    "id": 1,
    "type": 1,
    "date": "2023-01-15T10:30:00Z",
    "amount": 100.50,
    "cpf": "12345678901",
    "card": "123456789012",
    "time": "10:30:00",
    "storeOwner": "JOÃO MACEDO",
    "storeName": "BAR DO JOÃO",
    "storeId": 1,
    "typeDescription": "Debit",
    "nature": "In",
    "sign": "+"
  }
]
```

## 🔌 How to Consume the API

### 📋 **Base URLs**
```
CNAB Processing: http://localhost:5000/api/cnab
Store Operations: http://localhost:5000/api/stores
```

### 🚀 **1. Upload CNAB File**

#### **cURL:**
```bash
curl -X POST "http://localhost:5000/api/cnab/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@cnab.txt"
```

#### **PowerShell:**
```powershell
$uri = "http://localhost:5000/api/cnab/upload"
$filePath = "C:\path\to\cnab.txt"

$form = @{
    file = Get-Item $filePath
}

Invoke-RestMethod -Uri $uri -Method Post -Form $form
```

#### **Browser (HTML Form):**
```html
<form action="http://localhost:5000/api/cnab/upload" method="post" enctype="multipart/form-data">
    <input type="file" name="file" accept=".txt" required>
    <button type="submit">Upload CNAB File</button>
</form>
```

#### **Response Example:**
```json
{
  "success": true,
  "message": "File processed successfully. 3 stores processed, 15 transactions imported.",
  "processedStores": 3,
  "processedTransactions": 15
}
```

### 📊 **2. List All Stores**

#### **cURL:**
```bash
curl -X GET "http://localhost:5000/api/stores"
```

#### **PowerShell:**
```powershell
$uri = "http://localhost:5000/api/stores"
$response = Invoke-RestMethod -Uri $uri -Method Get
$response | ConvertTo-Json -Depth 3
```

#### **Browser:**
```
http://localhost:5000/api/stores
```

#### **Response Example:**
```json
[
  {
    "id": 1,
    "owner": "JOÃO MACEDO",
    "name": "BAR DO JOÃO",
    "balance": 1250.50,
    "transactionCount": 5
  },
  {
    "id": 2,
    "owner": "MARIA SILVA",
    "name": "LOJA DA MARIA",
    "balance": -500.25,
    "transactionCount": 3
  }
]
```

### 🔍 **3. Get Store Transactions**

#### **cURL:**
```bash
curl -X GET "http://localhost:5000/api/stores/1/transactions"
```

#### **PowerShell:**
```powershell
$uri = "http://localhost:5000/api/stores/1/transactions"
$response = Invoke-RestMethod -Uri $uri -Method Get
$response | ConvertTo-Json -Depth 3
```

#### **Browser:**
```
http://localhost:5000/api/stores/1/transactions
```

#### **Response Example:**
```json
[
  {
    "id": 1,
    "type": 1,
    "date": "2023-01-15T10:30:00Z",
    "amount": 100.50,
    "cpf": "12345678901",
    "card": "123456789012",
    "time": "10:30:00",
    "storeOwner": "JOÃO MACEDO",
    "storeName": "BAR DO JOÃO",
    "storeId": 1,
    "typeDescription": "Debit",
    "nature": "In",
    "sign": "+"
  },
  {
    "id": 2,
    "type": 2,
    "date": "2023-01-15T14:20:00Z",
    "amount": 50.00,
    "cpf": "98765432109",
    "card": "987654321098",
    "time": "14:20:00",
    "storeOwner": "JOÃO MACEDO",
    "storeName": "BAR DO JOÃO",
    "storeId": 1,
    "typeDescription": "Boleto",
    "nature": "Out",
    "sign": "-"
  }
]
```

### 🌐 **4. Web Interface**

#### **Main Page:**
```
http://localhost:5000
```
- Upload CNAB files via web form
- View stores and transactions
- Interactive interface

#### **API Documentation (Swagger):**
```
http://localhost:5000/swagger
```
- Interactive API documentation
- Test endpoints directly
- View request/response schemas


### 📝 **5. HTTP Response Codes**

| Code | Description | Example |
|------|-------------|---------|
| `200` | Success | File processed successfully |
| `400` | Bad Request | Invalid file or incorrect format |
| `500` | Internal Server Error | Internal server error |

### 🎯 **6. Testing the API**

#### **Via Browser:**
1. **Main Interface**: http://localhost:5000
2. **API Documentation**: http://localhost:5000/swagger
3. **Direct API Calls**: 
   - http://localhost:5000/api/stores
   - http://localhost:5000/api/stores/1/transactions

#### **Via Command Line:**

**Windows (PowerShell):**
```powershell
# Upload file
$file = Get-Item "C:\path\to\cnab.txt"
$form = @{file = $file}
Invoke-RestMethod -Uri "http://localhost:5000/api/cnab/upload" -Method Post -Form $form

# Get stores
Invoke-RestMethod -Uri "http://localhost:5000/api/stores" -Method Get

# Get store transactions
Invoke-RestMethod -Uri "http://localhost:5000/api/stores/1/transactions" -Method Get
```

**Linux/macOS:**
```bash
# Upload file
curl -X POST "http://localhost:5000/api/cnab/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@cnab.txt"

# Get stores
curl -X GET "http://localhost:5000/api/stores"

# Get store transactions
curl -X GET "http://localhost:5000/api/stores/1/transactions"
```

#### **Via Postman:**
1. **POST** `http://localhost:5000/api/cnab/upload`
   - Body: form-data
   - Key: `file`, Type: File, Value: select .txt file
2. **GET** `http://localhost:5000/api/stores`
   - View the list of stores
3. **GET** `http://localhost:5000/api/stores/{storeId}/transactions`
   - View transactions for a specific store

### 🔧 **7. Complete Workflow Example**

#### **Step 1: Upload CNAB File**
```bash
curl -X POST "http://localhost:5000/api/cnab/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@cnab.txt"
```

#### **Step 2: List All Stores**
```bash
curl -X GET "http://localhost:5000/api/stores"
```

#### **Step 3: Get Transactions for Store ID 1**
```bash
curl -X GET "http://localhost:5000/api/stores/1/transactions"
```

#### **Step 4: Get Transactions for Store ID 2**
```bash
curl -X GET "http://localhost:5000/api/stores/2/transactions"
```

## 🧪 Tests

Run unit tests:

```bash
dotnet test
```

### Test Coverage

- ✅ CNAB file parser
- ✅ Data models (Transaction, Store)
- ✅ Store balance calculation
- ✅ Transaction type validation

## 📝 CNAB Format

The system processes CNAB files with the following format:

| Field | Position | Size | Description |
|-------|----------|------|-------------|
| Type | 1-1 | 1 | Transaction type |
| Date | 2-9 | 8 | Date (YYYYMMDD) |
| Value | 10-19 | 10 | Value (divide by 100) |
| CPF | 20-30 | 11 | Beneficiary CPF |
| Card | 31-42 | 12 | Card used |
| Time | 43-48 | 6 | Time (HHMMSS) |
| Store Owner | 49-62 | 14 | Representative name |
| Store Name | 63-81 | 19 | Store name |

### Transaction Types

| Type | Description | Nature | Sign |
|------|-------------|--------|------|
| 1 | Debit | In | + |
| 2 | Boleto | Out | - |
| 3 | Financing | Out | - |
| 4 | Credit | In | + |
| 5 | Loan Receipt | In | + |
| 6 | Sales | In | + |
| 7 | TED Receipt | In | + |
| 8 | DOC Receipt | In | + |
| 9 | Rent | Out | - |

## 🐳 Docker

### 🚀 How to Run the Application with Docker

### ⚠️ Prerequisites

Before running `docker-compose up --build`, make sure you have:

#### 1. Docker and Docker Compose Installed

**Windows:**
```bash
# Check if Docker is installed
docker --version
docker-compose --version

# If not installed, download Docker Desktop
# https://www.docker.com/products/docker-desktop/
```

**macOS:**
```bash
# Check if Docker is installed
docker --version
docker-compose --version

# If not installed, download Docker Desktop
# https://www.docker.com/products/docker-desktop/
```

**Linux (Ubuntu/Debian):**
```bash
# Install Docker
sudo apt update
sudo apt install docker.io docker-compose

# Add user to docker group
sudo usermod -aG docker $USER
# (Restart terminal after this command)

# Verify installation
docker --version
docker-compose --version
```

**Linux (CentOS/RHEL):**
```bash
# Install Docker
sudo yum install docker docker-compose

# Start and enable Docker
sudo systemctl start docker
sudo systemctl enable docker

# Add user to docker group
sudo usermod -aG docker $USER
# (Restart terminal after this command)
```

**macOS (Apple Silicon M1/M2):**
```bash
# Check architecture
uname -m
# Should return: arm64

# Docker Desktop for Apple Silicon already includes ARM64 support
# Download from: https://www.docker.com/products/docker-desktop/

# Verify it's working
docker --version
docker-compose --version
```

**Linux (ARM64 - Raspberry Pi, etc.):**
```bash
# Install Docker for ARM64
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add user to docker group
sudo usermod -aG docker $USER

# Install Docker Compose
sudo apt install docker-compose
```

#### 2. Check System Resources

**Minimum recommended:**
- **RAM**: 4GB (8GB recommended)
- **Disk space**: 2GB free
- **CPU**: 2 cores

**Check resources:**
```bash
# Linux/macOS
free -h          # Check RAM
df -h            # Check disk space
nproc            # Check number of cores

# Windows (PowerShell)
Get-ComputerInfo | Select-Object TotalPhysicalMemory,NumberOfProcessors
Get-WmiObject -Class Win32_LogicalDisk | Select-Object Size,FreeSpace
```

#### 3. Check Available Ports

**Check if ports are free:**
```bash
# Linux/macOS
netstat -tulpn | grep :8080
netstat -tulpn | grep :5432

# Windows
netstat -an | findstr :8080
netstat -an | findstr :5432
```

**If ports are in use, stop the services:**
```bash
# Stop services that might be using the ports
sudo systemctl stop apache2    # If running
sudo systemctl stop nginx       # If running
sudo systemctl stop postgresql  # If running
```

#### 4. Check Permissions (Linux/macOS)

```bash
# Check if user has Docker permission
docker ps

# If permission error:
sudo chmod 666 /var/run/docker.sock
# OR
sudo usermod -aG docker $USER
# (Restart terminal)
```

### 🚀 How to Run the Application with Docker

#### Option 1: Docker Compose (Recommended)

##### 1. Pre-flight Check (Recommended)
```bash
# Check if Docker is working
docker --version
docker-compose --version

# Check if ports are free
netstat -tulpn | grep :8080 || echo "Port 8080 free"
netstat -tulpn | grep :5432 || echo "Port 5432 free"

# Check disk space (minimum 2GB)
df -h | head -1
df -h | grep -E "(/$|/home)" | awk '{print "Free space: " $4}'
```

##### 2. Run the entire application
```bash
# In the project root, execute:
docker-compose up -d
```

This command will:
- ✅ Create and configure PostgreSQL database
- ✅ Build the .NET application image
- ✅ Run migrations automatically
- ✅ Start the web application

##### 3. Check if containers are running
```bash
docker-compose ps
```

You should see something like:
```
Name                     Command               State           Ports
-------------------------------------------------------------------------------
test-cnab-app-1         dotnet CnabProcessor.dll   Up      0.0.0.0:8080->8080/tcp
test-cnab-postgres-1     docker-entrypoint.sh postgres   Up      0.0.0.0:5432->5432/tcp
```

##### 4. Access the application
- **🌐 Web Interface**: http://localhost:8080
- **📊 API Swagger**: http://localhost:8080/swagger
- **🗄️ PostgreSQL Database**: localhost:5432

##### 5. Stop the application
```bash
docker-compose down
```

##### 6. Stop and remove volumes (clean data)
```bash
docker-compose down -v
```

#### Option 2: Individual Docker

##### 1. Run only PostgreSQL database
```bash
docker run --name cnab-postgres \
  -e POSTGRES_PASSWORD=password \
  -e POSTGRES_DB=cnabdb \
  -p 5432:5432 \
  -d postgres:15
```

##### 2. Build the application image
```bash
cd CnabProcessor
docker build -t cnab-processor .
```

##### 3. Run the application
```bash
docker run --name cnab-app \
  --link cnab-postgres:postgres \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=cnabdb;Username=postgres;Password=password" \
  -p 8080:8080 \
  -d cnab-processor
```

##### 4. Run migrations
```bash
docker exec cnab-app dotnet ef database update
```

### 🔧 Useful Docker Commands

#### View application logs
```bash
docker-compose logs -f app
```

#### View database logs
```bash
docker-compose logs -f postgres
```

#### Enter application container
```bash
docker-compose exec app bash
```

#### Enter PostgreSQL database
```bash
docker-compose exec postgres psql -U postgres -d cnab_processor
```

#### Restart only the application
```bash
docker-compose restart app
```

#### Rebuild and start again
```bash
docker-compose up --build -d
```

### 🐛 Troubleshooting

#### ❌ Common Problems and Solutions

##### 1. "docker-compose: command not found"
**Problem**: Docker Compose is not installed
**Solution**:
```bash
# Linux (Ubuntu/Debian)
sudo apt install docker-compose

# Linux (CentOS/RHEL)
sudo yum install docker-compose

# macOS/Windows
# Install Docker Desktop which includes Docker Compose
```

##### 2. "Permission denied" (Linux/macOS)
**Problem**: User doesn't have Docker permission
**Solution**:
```bash
# Add user to docker group
sudo usermod -aG docker $USER

# Restart terminal or logout/login
# OR
sudo chmod 666 /var/run/docker.sock
```

##### 3. "Port already in use"
**Problem**: Ports 8080 or 5432 are already in use
**Solution**:
```bash
# Check what's using the ports
sudo lsof -i :8080
sudo lsof -i :5432

# Stop conflicting services
sudo systemctl stop apache2
sudo systemctl stop nginx
sudo systemctl stop postgresql

# OR use different ports in docker-compose.yml
```

##### 4. "No space left on device"
**Problem**: Disk full
**Solution**:
```bash
# Clean unused Docker images
docker system prune -a

# Clean unused volumes
docker volume prune

# Check disk space
df -h
```

##### 5. "Cannot connect to Docker daemon"
**Problem**: Docker daemon is not running
**Solution**:
```bash
# Linux
sudo systemctl start docker
sudo systemctl enable docker

# macOS/Windows
# Start Docker Desktop
```

##### 6. "Build failed" - Compilation error
**Problem**: Error building .NET image
**Solution**:
```bash
# Clean Docker cache
docker builder prune

# Rebuild without cache
docker-compose build --no-cache

# Check if Dockerfile is correct
cat CnabProcessor/Dockerfile
```

##### 7. "Database connection failed"
**Problem**: Application cannot connect to database
**Solution**:
```bash
# Check if database is running
docker-compose ps postgres

# Check database logs
docker-compose logs postgres

# Wait for database to initialize (may take a few seconds)
sleep 10
docker-compose restart app
```

##### 8. "Out of memory" (OOM)
**Problem**: System without enough memory
**Solution**:
```bash
# Check memory usage
docker stats

# Limit memory usage in docker-compose.yml
# Add: mem_limit: 1g
```

#### 🔧 Diagnostic Commands

##### Check container status:
```bash
docker-compose ps
```

##### View logs in real-time:
```bash
# Application logs
docker-compose logs -f app

# Database logs
docker-compose logs -f postgres

# All logs
docker-compose logs -f
```

##### Check system resources:
```bash
# CPU and memory usage
docker stats

# Disk space
df -h

# Available memory
free -h
```

##### Test connectivity:
```bash
# Test if application responds
curl http://localhost:8080

# Test if database is accessible
docker-compose exec app ping postgres
```

#### 🧹 Complete Cleanup

##### If nothing works, clean everything:
```bash
# Stop and remove everything
docker-compose down -v

# Remove images
docker rmi $(docker images -q)

# Remove volumes
docker volume prune -f

# Remove networks
docker network prune -f

# Clean system
docker system prune -af

# Rebuild from scratch
docker-compose up --build -d
```

#### 📞 Support by Operating System

##### Windows:
- Make sure Docker Desktop is running
- Check if WSL2 is enabled (Windows 10/11)
- Run as administrator if necessary

##### macOS:
- Make sure Docker Desktop is running
- Check if Docker has proper permissions
- On M1/M2 Macs, use ARM64 compatible images

##### Linux:
- Make sure Docker daemon is running
- Check user permissions
- In some cases, you may need to use `sudo`

### 📝 Environment Variables

The `docker-compose.yml` file is already configured with the following variables:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=cnabdb;Username=postgres;Password=password
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://+:8080
```

### 🌐 Access Points

After running with Docker Compose:

- **🏠 Main Page**: http://localhost:8080
- **📊 API Swagger**: http://localhost:8080/swagger
- **📁 CNAB Upload**: http://localhost:8080 (web interface)
- **🗄️ PostgreSQL**: localhost:5432 (user: postgres, password: password)

### Dockerfile
The project includes a production-optimized Dockerfile with multi-stage build.

### Docker Compose
Complete configuration with PostgreSQL and application:

```yaml
services:
  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: cnab_processor
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"

  app:
    build: ./CnabProcessor
    ports:
      - "5000:8080"
    depends_on:
      - postgres
```

## 🔧 Development

### Code Standards
- C# naming conventions
- Async/await for I/O operations
- Dependency Injection
- Repository pattern (via EF Core)
- Clean Architecture principles

## 📈 Performance

- Asynchronous file processing
- **EF Core Bulk Extensions** for high-performance transaction insertion
- Optimized database indexes
- Connection pooling
- **10-100x faster** transaction processing compared to individual inserts

## 🔒 Security

- File type validation
- Input data sanitization
- Prepared statements (EF Core)
- HTTPS in production
