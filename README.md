# Omani Legal AI Chat System

## Overview

The Omani Legal AI Chat System is a sophisticated consultation platform that enables users to interact with an AI-powered legal assistant trained on Omani law. The system leverages a knowledge base of user-uploaded documents and provides accurate, document-based legal consultations.

## Key Features

- **Smart Chat Interface**: Create conversation rooms, send messages, and receive AI-powered legal consultations
- **Document-Bounded Responses**: AI responses are limited to information from the knowledge base only
- **Document Management**: Upload, manage, and analyze legal documents (PDF files)
- **Conversation Tracking**: Track and analyze consultation history
- **Relevance Ranking**: Automatically find the most relevant documents for each query
- **Citation Support**: Responses include citations from the source documents
- **Subscription Management**: Tiered access to features based on subscription plans

## Architecture

The system follows a service-oriented architecture with the following key components:

### Core Services

- **ChatDbService**: Manages chat rooms and messages
- **KnowledgeBaseService**: Handles document retrieval and relevance ranking
- **DeepSeekService**: Interfaces with the DeepSeek AI model
- **PdfExtractionService**: Extracts and indexes text from PDF documents
- **ConversationTrackingService**: Records and analyzes conversations
- **FileManagementService**: Handles document uploads and storage
- **ChatRulesService**: Manages rules and guidelines for the AI assistant

### Data Flow

1. User uploads legal documents to their personal library
2. User creates a chat room and sends a query
3. System identifies relevant documents from the user's library
4. System enriches the query with content from relevant documents
5. AI generates a response based strictly on the provided document context
6. System tracks the conversation and document usage for analytics
7. Response is returned to user with document references

## Getting Started

### Prerequisites

- .NET 6.0 or higher
- SQL Server database
- DeepSeek API access

### Configuration

Configure the following settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_SQL_Connection_String"
  },
  "DeepSeekSettings": {
    "ApiUrl": "https://your-deepseek-endpoint.com",
    "ApiKey": "your-api-key",
    "Endpoint": "api/generate"
  },
  "FileStorage": {
    "BasePath": "Files",
    "PdfFolderName": "Documents",
    "MaxDocumentSizeMB": 20
  }
}
```

### Running the Application

1. Clone the repository
2. Set up the database using Entity Framework migrations
3. Configure the application settings
4. Build and run the application

## Usage

### Creating a Chat Room

```csharp
// Create a new chat room
long roomId = await _chatDbService.CreateChatRoomAsync(
    "Land Ownership Consultation", 
    "Consultation about land ownership regulations in Oman", 
    userId
);
```

### Sending a Query to the AI

```csharp
// Process a user query
var response = await _chatDbService.ProcessAIMessageAsync(
    roomId,
    userId,
    "What are the requirements for foreign land ownership in Muscat?",
    "ar" // or "en" for English
);
```

### Managing Documents

```csharp
// Upload a legal document
var result = await _fileManagementService.UploadPdfFileAsync(
    pdfFile,
    userId,
    "ar"
);
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- DeepSeek AI for providing the language model technology
- Omani Ministry of Legal Affairs for domain expertise consultation