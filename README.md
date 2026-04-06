## SUTESFOZES

SUTESFOZES is a community-driven recipe sharing platform where anyone can upload and share their own cooking and baking recipes - completely ad-free and distraction-free.

Inspired by communities like Reddit’s cooking subreddits, but built as a standalone modern web application.

# Features
Registration & Login

Settings (e.g. email change, username change, password change)

My Recipes and Favourite Recipes

Submit a Recipe

Search Recipes

# Tech Stack
Backend: C# (.NET)
Frontend: React + Vue
Database: PostgreSQL
File Storage: Cloudflare R2

# Performance & Optimization
Lazy loading for all images
Session Storage-based image caching


## Project Setup

# Frontend Setup
1. Navigate to the frontend directory
2. Create and configure your .env file:
```VITE_API_URL=your_api_url```

3. Install dependencies and build:
```
npm install
npm run build
```

# Backend Setup

Build for Linux:
```dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true```
Build for Windows:
```dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true```

Backend .env Configuration

After building, create a .env file in the output directory:
```
PRODUCT_LINK=product_link
JWT_SECRET=jwt_secret

PSQL_CONNECTIONSTRING="Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=sutesfozes;Pooling=true;MaxPoolSize=100;Timeout=5;"

SMTP_HOSTNAME=hostname
SMTP_HOSTPORT=port
SMTP_USERNAME=username
SMTP_PASSWORD=password

R2_ACCOUNTID=id
R2_BUCKET=bucket
R2_ACCESS=access
R2_SECRET=secret
R2_BUCKET_ID=bucket_id
```

# Database Setup
Create a PostgreSQL database:
sutesfozes
Run the schema located in:
data.sql
