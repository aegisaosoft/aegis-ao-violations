# Voice Booking System - Implementation Guide

## Overview

This voice-controlled booking system allows users to complete a full car rental booking using only their voice. The system includes:

1. **Vehicle Selection** - AI-powered recommendations based on voice description
2. **Date Selection** - Natural language date parsing
3. **Customer Information** - Voice input for personal details
4. **Booking Review & Confirmation** - Summary and final confirmation

## Features

### 🎤 Voice Input
- Continuous speech recognition
- Natural language understanding
- Multi-language support (English, Russian, Portuguese, etc.)
- Fallback to manual input

### 🔊 Voice Output
- Text-to-speech responses
- Multiple voice options (male/female)
- Adjustable voice selection
- Real-time speaking indicator

### 🧠 Smart Parsing
- Date recognition ("tomorrow", "next Monday", "December 15")
- Duration parsing ("3 days", "one week")
- Email extraction
- Phone number extraction
- Name parsing

### ✨ User Experience
- Progress indicator
- Step-by-step guidance
- Visual feedback
- Manual input fallback for all fields
- Responsive design

## Installation

### 1. Copy Files

Copy these files to your React project:

```bash
src/
├── components/
│   ├── VoiceBooking.js
│   └── VoiceBooking.css
```

### 2. Install Dependencies

The component uses these existing dependencies:
- React 18+
- react-i18next (for translations)
- lucide-react (for icons)
- Your existing CompanyContext

### 3. Add to Your App

```javascript
import VoiceBooking from './components/VoiceBooking';

function App() {
  const [availableVehicles, setAvailableVehicles] = useState([]);

  const handleBookingComplete = (bookingResult) => {
    console.log('Booking completed:', bookingResult);
    // Navigate to confirmation page or show success message
  };

  return (
    <VoiceBooking 
      availableVehicles={availableVehicles}
      onBookingComplete={handleBookingComplete}
    />
  );
}
```

## API Requirements

### 1. Recommendations Endpoint

**Endpoint:** `POST /api/recommendations/ai`

**Request:**
```json
{
  "requirements": "I need a family car for 5 people",
  "language": "en",
  "availableVehicles": [...],
  "mode": "claude"
}
```

**Response:**
```json
{
  "result": {
    "summary": "Based on your needs...",
    "recommendations": [
      {
        "vehicleId": "123",
        "make": "Toyota",
        "model": "Highlander",
        "matchScore": 95,
        "reasoning": "Perfect for families...",
        "totalCost": 450.00,
        "pros": ["Spacious", "Safe"],
        "cons": ["Higher price"]
      }
    ]
  }
}
```

### 2. Booking Endpoint

**Endpoint:** `POST /api/bookings`

**Request:**
```json
{
  "vehicleId": "123",
  "pickupDate": "2024-12-15T00:00:00Z",
  "returnDate": "2024-12-22T00:00:00Z",
  "customer": {
    "firstName": "John",
    "lastName": "Smith",
    "email": "john@example.com",
    "phone": "555-1234",
    "licenseNumber": "DL12345678"
  },
  "extras": {
    "insurance": false,
    "gps": false,
    "childSeat": false
  },
  "companyId": "company-uuid"
}
```

**Response:**
```json
{
  "success": true,
  "bookingId": "booking-123",
  "confirmationNumber": "ABC123456",
  "message": "Booking created successfully"
}
```

## Translation Keys

Add these keys to your i18n translation files:

```json
{
  "voiceBooking": {
    "stepVehicle": "Step 1: Choose Your Vehicle",
    "stepDates": "Step 2: Choose Your Dates",
    "stepCustomer": "Step 3: Your Information",
    "stepReview": "Step 4: Review & Confirm",
    "listening": "Listening...",
    "speaking": "Speaking...",
    "clickToSpeak": "Click to Speak",
    "youSaid": "You said:",
    "assistantVoice": "Assistant Voice:",
    "vehicle": "Vehicle",
    "dates": "Dates",
    "info": "Info",
    "review": "Review",
    "continue": "Continue",
    "confirmBooking": "Confirm Booking",
    "startOver": "Start Over",
    "bookingComplete": "Booking Complete!",
    "thankYou": "Thank you for your booking!",
    "confirmationSent": "A confirmation email has been sent to {{email}}",
    
    "vehicleInstruction": "Describe what type of vehicle you need...",
    "pickupInstruction": "When would you like to pick up the vehicle?",
    "returnInstruction": "When would you like to return it?",
    "nameInstruction": "Please tell me your first and last name.",
    "emailInstruction": "What is your email address?",
    "phoneInstruction": "What is your phone number?",
    "licenseInstruction": "What is your driver's license number?",
    "confirmInstruction": "Say 'confirm' to complete your booking...",
    
    "searchingVehicles": "Searching for vehicles...",
    "vehicleSelected": "I recommend the {{make}} {{model}}...",
    "askDates": "When would you like to pick up the vehicle?",
    "pickupConfirmed": "Pickup date set to {{date}}...",
    "datesConfirmed": "Perfect! You'll rent the vehicle for {{days}} days...",
    "nameConfirmed": "Thank you, {{name}}. What is your email?",
    "emailConfirmed": "Email saved. What is your phone number?",
    "phoneConfirmed": "Phone saved. What is your license number?",
    "licenseConfirmed": "License saved. Let me show you the summary.",
    
    "processing": "Processing your booking...",
    "success": "Booking completed successfully!",
    "error": "Sorry, I encountered an error.",
    "noVehiclesFound": "No vehicles found matching your needs.",
    "dateNotUnderstood": "I didn't understand that date.",
    "invalidReturnDate": "That return date doesn't work.",
    
    "orSelectManually": "Or select manually:",
    "selectVehicle": "Select a vehicle...",
    "selectedVehicle": "Selected Vehicle:",
    "firstName": "First Name:",
    "lastName": "Last Name:",
    "email": "Email:",
    "phone": "Phone:",
    "license": "Driver's License:",
    "reviewBooking": "Review Booking",
    "pickup": "Pickup",
    "return": "Return",
    "days": "days",
    "customerInfo": "Customer Information",
    "licenseNum": "License",
    "totalCost": "Total Cost"
  }
}
```

## Voice Commands Examples

### Step 1: Vehicle Selection
- "I need a family car for 5 people"
- "I want a luxury sedan"
- "Show me SUVs"
- "I need something economical"
- "Give me a car for a beach trip"

### Step 2: Date Selection

**Pickup Date:**
- "Tomorrow"
- "Next Monday"
- "December 15"
- "15th of December"
- "12/15/2024"

**Return Date:**
- "December 22"
- "3 days later"
- "One week after"
- "5 days"

### Step 3: Customer Information

**Name:**
- "John Smith"
- "Maria Garcia"

**Email:**
- "john dot smith at gmail dot com"
- "maria.garcia@company.com"

**Phone:**
- "555-1234"
- "five five five one two three four"
- "+1 555 123 4567"

**License:**
- "DL 12345678"
- "A B C one two three four five six"

### Step 4: Review

- "Confirm" or "Yes" - Complete booking
- "Change" or "Edit" - Modify details
- "Change vehicle" - Go back to vehicle selection
- "Change dates" - Go back to date selection
- "Change information" - Go back to customer info

## Browser Support

### Speech Recognition
✅ Chrome/Edge (best support)
✅ Safari (iOS 14.5+)
⚠️ Firefox (limited support)
❌ Internet Explorer

### Speech Synthesis
✅ All modern browsers
✅ Mobile devices (iOS, Android)

### Fallback
All features have manual input fallback, so the component works even without speech support.

## Customization

### Change Voice Settings

```javascript
// In VoiceBooking.js, modify speech settings:
utterance.rate = 0.9;  // Speed (0.1 to 10)
utterance.pitch = 1.0; // Pitch (0 to 2)
utterance.volume = 1.0; // Volume (0 to 1)
```

### Add More Date Formats

```javascript
// In parseDate function, add more patterns:
const datePatterns = [
  /your-pattern-here/,
  // ... existing patterns
];
```

### Customize Voices

```javascript
// In categorizedVoices useMemo, add more voice categories:
if (name.includes('child') || name.includes('kid')) {
  categories.child.push(voice);
}
```

## Troubleshooting

### Issue: Voice recognition not working
**Solution:** 
- Check browser support
- Ensure HTTPS (required for microphone access)
- Check microphone permissions
- Use Chrome/Edge for best support

### Issue: Voice synthesis not speaking
**Solution:**
- Check volume settings
- Ensure voices are loaded (check availableVoices)
- Try different voice selection
- Check browser console for errors

### Issue: Dates not parsing correctly
**Solution:**
- Add console.log to parseDate function
- Add more date patterns for your language
- Use manual date input as fallback

### Issue: API errors
**Solution:**
- Check API endpoint URLs
- Verify request/response format
- Check CORS settings
- Review backend logs

## Performance Tips

1. **Lazy Loading:** Load the component only when needed
2. **Voice Caching:** Selected voice is saved in state
3. **API Optimization:** Debounce API calls if needed
4. **Cleanup:** Component properly cleans up speech synthesis

## Security Considerations

1. **Input Validation:** All voice input is validated before API calls
2. **Sanitization:** Customer data is sanitized
3. **HTTPS Required:** Microphone access requires secure context
4. **Rate Limiting:** Consider rate limiting API calls

## Future Enhancements

- [ ] Add voice authentication
- [ ] Support for multiple languages simultaneously
- [ ] Emotion detection in voice
- [ ] Background noise cancellation
- [ ] Booking history via voice
- [ ] Payment processing via voice
- [ ] Vehicle comparison via voice
- [ ] Location selection via voice

## Support

For issues or questions:
1. Check browser console for errors
2. Review API logs
3. Test with manual input fallback
4. Check translation keys are loaded
5. Verify API endpoints are accessible

## Example Usage

```javascript
import React, { useState, useEffect } from 'react';
import VoiceBooking from './components/VoiceBooking';

function BookingPage() {
  const [vehicles, setVehicles] = useState([]);

  useEffect(() => {
    // Load available vehicles
    fetch('/api/vehicles')
      .then(res => res.json())
      .then(data => setVehicles(data));
  }, []);

  const handleBookingComplete = (result) => {
    console.log('Booking confirmed:', result);
    // Redirect or show success
    window.location.href = '/booking-confirmation/' + result.bookingId;
  };

  return (
    <div className="booking-page">
      <h1>Voice Booking</h1>
      <VoiceBooking 
        availableVehicles={vehicles}
        onBookingComplete={handleBookingComplete}
      />
    </div>
  );
}

export default BookingPage;
```

## License

This component is part of your car rental system and follows your project's license.
