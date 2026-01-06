# Toolvia

A collection of useful online tools for developers and designers. Built with Vue.js 3 and .NET 10 API.

## Features

### Encoding
- **Base64** - Encode/decode Base64
- **Hex** - Encode/decode Hexadecimal
- **URL** - Encode/decode URL components
- **HTML** - Encode/decode HTML entities

### Hash
- **MD5** - Generate MD5 hash
- **SHA-1** - Generate SHA-1 hash
- **SHA-256** - Generate SHA-256 hash
- **SHA-512** - Generate SHA-512 hash

### Converter
- **Unit Converter** - Convert between units (length, weight, temperature, etc.)
- **Case Converter** - Convert text case (camelCase, snake_case, etc.)
- **Number Base** - Convert between binary, octal, decimal, hexadecimal
- **Color Converter** - Convert between HEX, RGB, HSL

### Format
- **JSON Formatter** - Format, validate, minify, and unescape JSON
- **XML Formatter** - Format, validate, and minify XML
- **SQL Formatter** - Format SQL queries

### Generator
- **UUID Generator** - Generate random UUIDs
- **Password Generator** - Generate secure passwords
- **Lorem Ipsum** - Generate placeholder text

### Other
- **QR Code** - Generate QR codes
- **Text Diff** - Compare text differences

## Tech Stack

### Frontend
- Vue.js 3 + TypeScript
- Vite
- TailwindCSS v4
- Pinia (State Management)
- Vue Router

### Backend
- .NET 10
- ASP.NET Core Web API

## Getting Started

### Prerequisites
- Node.js 18+
- .NET 10 SDK

### Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

Frontend will be available at `http://localhost:5173`

### Backend Setup

```bash
cd backend
dotnet run
```

API will be available at `http://localhost:5000`

## Development

### Frontend Commands
- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build

### Backend Commands
- `dotnet run` - Start development server
- `dotnet build` - Build project
- `dotnet publish` - Publish for production

## Project Structure

```
MultiTools/
├── frontend/                # Vue.js frontend
│   ├── src/
│   │   ├── components/      # Reusable components
│   │   ├── views/           # Page components
│   │   │   └── tools/       # Tool pages
│   │   ├── stores/          # Pinia stores
│   │   ├── router/          # Vue Router config
│   │   ├── types/           # TypeScript types
│   │   └── utils/           # Utility functions
│   └── ...
├── backend/                 # .NET API
│   ├── Controllers/         # API controllers
│   ├── Models/              # Data models
│   └── ...
└── README.md
```

## License

MIT
