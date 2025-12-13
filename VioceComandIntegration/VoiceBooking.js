import React, { useCallback, useEffect, useMemo, useState, useRef } from 'react';
import { Mic, Volume2, Calendar, User, CreditCard, CheckCircle, AlertCircle } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { useCompany } from '../context/CompanyContext';
import './VoiceBooking.css';

/**
 * Voice-Controlled Booking Component
 * 
 * This component provides a complete voice-driven booking experience:
 * 1. Vehicle Selection - User describes needs, AI recommends vehicles
 * 2. Date Selection - Voice input for pickup/return dates
 * 3. Customer Information - Voice input for contact details
 * 4. Booking Confirmation - Review and complete booking
 * 
 * Features:
 * - Multi-language support
 * - Voice feedback (speaks back to user)
 * - Fallback to manual input
 * - Smart date parsing
 * - Error handling and validation
 */

const VoiceBooking = ({ availableVehicles = [], onBookingComplete }) => {
  const { t, i18n } = useTranslation();
  const { formatPrice, companyId } = useCompany();

  // Booking flow steps
  const STEPS = {
    VEHICLE: 'vehicle',
    DATES: 'dates',
    CUSTOMER: 'customer',
    REVIEW: 'review',
    COMPLETE: 'complete'
  };

  // Component state
  const [currentStep, setCurrentStep] = useState(STEPS.VEHICLE);
  const [isListening, setIsListening] = useState(false);
  const [transcript, setTranscript] = useState('');
  const [isSpeaking, setIsSpeaking] = useState(false);
  const [selectedVoice, setSelectedVoice] = useState(null);
  const [availableVoices, setAvailableVoices] = useState([]);
  
  // Booking data
  const [bookingData, setBookingData] = useState({
    vehicle: null,
    pickupDate: null,
    returnDate: null,
    customer: {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      licenseNumber: ''
    },
    extras: {
      insurance: false,
      gps: false,
      childSeat: false
    }
  });

  // Speech recognition reference
  const recognitionRef = useRef(null);
  const synthRef = useRef(window.speechSynthesis);

  /**
   * Initialize speech recognition
   * Sets up browser's speech recognition API
   */
  useEffect(() => {
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    
    if (SpeechRecognition) {
      const recognition = new SpeechRecognition();
      recognition.continuous = false;
      recognition.interimResults = false;
      recognition.lang = i18n.language;

      recognition.onresult = (event) => {
        const result = event.results[0][0].transcript;
        setTranscript(result);
        handleVoiceCommand(result);
      };

      recognition.onerror = (event) => {
        console.error('Speech recognition error:', event.error);
        setIsListening(false);
      };

      recognition.onend = () => {
        setIsListening(false);
      };

      recognitionRef.current = recognition;
    }

    return () => {
      if (recognitionRef.current) {
        recognitionRef.current.abort();
      }
    };
  }, [i18n.language]);

  /**
   * Load available voices for text-to-speech
   */
  useEffect(() => {
    const loadVoices = () => {
      const voices = synthRef.current.getVoices();
      if (voices.length > 0) {
        setAvailableVoices(voices);
        
        // Set default voice based on language
        if (!selectedVoice) {
          const langCode = i18n.language.split('-')[0];
          const defaultVoice = 
            voices.find(v => v.lang.startsWith(langCode) && v.name.includes('Female')) ||
            voices.find(v => v.lang.startsWith(langCode)) ||
            voices[0];
          setSelectedVoice(defaultVoice);
        }
      }
    };

    loadVoices();
    
    if (synthRef.current.onvoiceschanged !== undefined) {
      synthRef.current.onvoiceschanged = loadVoices;
    }
  }, [i18n.language, selectedVoice]);

  /**
   * Text-to-speech function
   * Speaks text back to user with selected voice
   */
  const speak = useCallback((text) => {
    if (!text) return;
    
    synthRef.current.cancel();
    const utterance = new SpeechSynthesisUtterance(text);
    
    if (selectedVoice) {
      utterance.voice = selectedVoice;
    }
    
    utterance.rate = 0.9; // Slightly slower for clarity
    utterance.pitch = 1.0;
    utterance.volume = 1.0;
    
    utterance.onstart = () => setIsSpeaking(true);
    utterance.onend = () => setIsSpeaking(false);
    utterance.onerror = () => setIsSpeaking(false);
    
    synthRef.current.speak(utterance);
  }, [selectedVoice]);

  /**
   * Stop speaking
   */
  const stopSpeaking = useCallback(() => {
    synthRef.current.cancel();
    setIsSpeaking(false);
  }, []);

  /**
   * Start/stop listening
   */
  const toggleListening = useCallback(() => {
    if (!recognitionRef.current) {
      alert('Speech recognition is not supported in this browser.');
      return;
    }

    if (isListening) {
      recognitionRef.current.stop();
      setIsListening(false);
    } else {
      setTranscript('');
      recognitionRef.current.start();
      setIsListening(true);
    }
  }, [isListening]);

  /**
   * Parse dates from natural language
   * Examples: "tomorrow", "next Monday", "December 15", "15/12/2024"
   */
  const parseDate = (text) => {
    const lowerText = text.toLowerCase();
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    // Tomorrow
    if (lowerText.includes('tomorrow')) {
      const tomorrow = new Date(today);
      tomorrow.setDate(tomorrow.getDate() + 1);
      return tomorrow;
    }

    // Next week
    if (lowerText.includes('next week')) {
      const nextWeek = new Date(today);
      nextWeek.setDate(nextWeek.getDate() + 7);
      return nextWeek;
    }

    // Try to find date patterns (15 December, Dec 15, 12/15, etc)
    const datePatterns = [
      /(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{2,4})/, // 15/12/2024 or 15-12-24
      /(\d{1,2})\s+(january|february|march|april|may|june|july|august|september|october|november|december)/i,
      /(january|february|march|april|may|june|july|august|september|october|november|december)\s+(\d{1,2})/i
    ];

    for (const pattern of datePatterns) {
      const match = text.match(pattern);
      if (match) {
        try {
          const parsed = new Date(match[0]);
          if (!isNaN(parsed.getTime())) {
            return parsed;
          }
        } catch (e) {
          console.error('Date parse error:', e);
        }
      }
    }

    return null;
  };

  /**
   * Parse duration from text
   * Examples: "3 days", "one week", "5 days"
   */
  const parseDuration = (text) => {
    const lowerText = text.toLowerCase();
    
    // Look for number + days/weeks
    const dayMatch = lowerText.match(/(\d+|one|two|three|four|five|six|seven)\s+days?/);
    if (dayMatch) {
      const numberWords = { one: 1, two: 2, three: 3, four: 4, five: 5, six: 6, seven: 7 };
      const days = numberWords[dayMatch[1]] || parseInt(dayMatch[1]);
      return days;
    }

    const weekMatch = lowerText.match(/(\d+|one|two|three|four)\s+weeks?/);
    if (weekMatch) {
      const numberWords = { one: 1, two: 2, three: 3, four: 4 };
      const weeks = numberWords[weekMatch[1]] || parseInt(weekMatch[1]);
      return weeks * 7;
    }

    return null;
  };

  /**
   * Extract email from text
   */
  const extractEmail = (text) => {
    const emailPattern = /[\w\.-]+@[\w\.-]+\.\w+/;
    const match = text.match(emailPattern);
    return match ? match[0] : null;
  };

  /**
   * Extract phone from text
   */
  const extractPhone = (text) => {
    // Remove common words that might interfere
    const cleaned = text.replace(/\b(phone|number|is|my)\b/gi, '');
    
    // Look for phone patterns
    const phonePattern = /(\+?\d{1,3}[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}/;
    const match = cleaned.match(phonePattern);
    return match ? match[0].replace(/\s+/g, '') : null;
  };

  /**
   * Handle voice commands based on current step
   */
  const handleVoiceCommand = async (command) => {
    const lowerCommand = command.toLowerCase();
    
    console.log(`Voice command received at step ${currentStep}:`, command);

    switch (currentStep) {
      case STEPS.VEHICLE:
        await handleVehicleSelection(command);
        break;
        
      case STEPS.DATES:
        await handleDateSelection(command);
        break;
        
      case STEPS.CUSTOMER:
        await handleCustomerInfo(command);
        break;
        
      case STEPS.REVIEW:
        await handleReviewCommands(command);
        break;
        
      default:
        break;
    }
  };

  /**
   * Step 1: Vehicle Selection
   * Uses AI to recommend vehicles based on voice description
   */
  const handleVehicleSelection = async (command) => {
    speak(t('voiceBooking.searchingVehicles', 'Searching for vehicles that match your needs...'));

    try {
      const response = await fetch('/api/recommendations/ai', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          requirements: command,
          language: i18n.language,
          availableVehicles: availableVehicles,
          mode: 'claude'
        }),
      });

      if (!response.ok) throw new Error('Failed to get recommendations');

      const data = await response.json();
      const actualData = data.result || data;

      if (actualData.recommendations && actualData.recommendations.length > 0) {
        const topVehicle = actualData.recommendations[0];
        
        // Find the full vehicle object
        const vehicle = availableVehicles.find(v => 
          v.id === topVehicle.vehicleId || 
          v.vehicleId === topVehicle.vehicleId ||
          v.vehicle_id === topVehicle.vehicleId
        );

        if (vehicle) {
          setBookingData(prev => ({ ...prev, vehicle }));
          
          speak(
            t('voiceBooking.vehicleSelected', 
              `I recommend the {{make}} {{model}}. It costs {{price}} per day. ${topVehicle.reasoning || ''}`,
              { 
                make: vehicle.make || vehicle.Make,
                model: vehicle.model || vehicle.Model,
                price: formatPrice(vehicle.dailyRate || vehicle.DailyRate)
              }
            )
          );

          // Auto-advance to dates after 3 seconds
          setTimeout(() => {
            setCurrentStep(STEPS.DATES);
            speak(t('voiceBooking.askDates', 'Great! Now, when would you like to pick up the vehicle? You can say something like "tomorrow" or "December 15th".'));
          }, 3000);
        }
      } else {
        speak(t('voiceBooking.noVehiclesFound', 'I couldn\'t find vehicles matching your needs. Please try describing your requirements differently.'));
      }
    } catch (error) {
      console.error('Vehicle selection error:', error);
      speak(t('voiceBooking.error', 'Sorry, I encountered an error. Please try again.'));
    }
  };

  /**
   * Step 2: Date Selection
   * Parses dates from natural language
   */
  const handleDateSelection = async (command) => {
    // Check if user is providing both dates or just pickup
    const hasPickup = !bookingData.pickupDate;
    const hasReturn = bookingData.pickupDate && !bookingData.returnDate;

    if (hasPickup) {
      const pickupDate = parseDate(command);
      
      if (pickupDate) {
        setBookingData(prev => ({ ...prev, pickupDate }));
        speak(
          t('voiceBooking.pickupConfirmed', 
            'Pickup date set to {{date}}. When would you like to return the vehicle?',
            { date: pickupDate.toLocaleDateString() }
          )
        );
      } else {
        speak(t('voiceBooking.dateNotUnderstood', 'I didn\'t understand that date. Please try again, for example: "tomorrow" or "December 15th".'));
      }
    } else if (hasReturn) {
      // Try to parse return date or duration
      let returnDate = parseDate(command);
      
      if (!returnDate) {
        // Try duration instead
        const duration = parseDuration(command);
        if (duration && bookingData.pickupDate) {
          returnDate = new Date(bookingData.pickupDate);
          returnDate.setDate(returnDate.getDate() + duration);
        }
      }

      if (returnDate && returnDate > bookingData.pickupDate) {
        const days = Math.ceil((returnDate - bookingData.pickupDate) / (1000 * 60 * 60 * 24));
        const totalCost = days * (bookingData.vehicle.dailyRate || bookingData.vehicle.DailyRate || 0);
        
        setBookingData(prev => ({ ...prev, returnDate }));
        
        speak(
          t('voiceBooking.datesConfirmed',
            'Perfect! You\'ll rent the vehicle for {{days}} days, from {{pickup}} to {{return}}. Total cost: {{total}}. Now I need some information about you.',
            {
              days,
              pickup: bookingData.pickupDate.toLocaleDateString(),
              return: returnDate.toLocaleDateString(),
              total: formatPrice(totalCost)
            }
          )
        );

        // Auto-advance to customer info
        setTimeout(() => {
          setCurrentStep(STEPS.CUSTOMER);
          speak(t('voiceBooking.askName', 'What is your first and last name?'));
        }, 4000);
      } else {
        speak(t('voiceBooking.invalidReturnDate', 'That return date doesn\'t work. Please provide a date after the pickup date.'));
      }
    }
  };

  /**
   * Step 3: Customer Information
   * Collects customer details via voice
   */
  const handleCustomerInfo = async (command) => {
    const { customer } = bookingData;

    // First name and last name
    if (!customer.firstName || !customer.lastName) {
      const names = command.trim().split(/\s+/);
      if (names.length >= 2) {
        setBookingData(prev => ({
          ...prev,
          customer: {
            ...prev.customer,
            firstName: names[0],
            lastName: names.slice(1).join(' ')
          }
        }));
        
        speak(
          t('voiceBooking.nameConfirmed',
            'Thank you, {{name}}. What is your email address?',
            { name: `${names[0]} ${names.slice(1).join(' ')}` }
          )
        );
      } else {
        speak(t('voiceBooking.nameNotUnderstood', 'Please provide both your first and last name.'));
      }
      return;
    }

    // Email
    if (!customer.email) {
      const email = extractEmail(command);
      if (email) {
        setBookingData(prev => ({
          ...prev,
          customer: { ...prev.customer, email }
        }));
        
        speak(
          t('voiceBooking.emailConfirmed',
            'Email saved as {{email}}. What is your phone number?',
            { email }
          )
        );
      } else {
        speak(t('voiceBooking.emailNotUnderstood', 'I didn\'t catch that email. Please spell it out clearly, for example: john dot smith at gmail dot com.'));
      }
      return;
    }

    // Phone
    if (!customer.phone) {
      const phone = extractPhone(command);
      if (phone) {
        setBookingData(prev => ({
          ...prev,
          customer: { ...prev.customer, phone }
        }));
        
        speak(
          t('voiceBooking.phoneConfirmed',
            'Phone number saved. What is your driver\'s license number?',
            { phone }
          )
        );
      } else {
        speak(t('voiceBooking.phoneNotUnderstood', 'I didn\'t understand that phone number. Please say it again, digit by digit if needed.'));
      }
      return;
    }

    // License number
    if (!customer.licenseNumber) {
      setBookingData(prev => ({
        ...prev,
        customer: { ...prev.customer, licenseNumber: command.toUpperCase() }
      }));
      
      speak(t('voiceBooking.licenseConfirmed', 'License number saved. Let me show you a summary of your booking.'));
      
      // Auto-advance to review
      setTimeout(() => {
        setCurrentStep(STEPS.REVIEW);
        speakBookingSummary();
      }, 2000);
    }
  };

  /**
   * Step 4: Review Commands
   * Handles confirmation or changes
   */
  const handleReviewCommands = async (command) => {
    const lowerCommand = command.toLowerCase();

    if (lowerCommand.includes('confirm') || lowerCommand.includes('book') || lowerCommand.includes('yes')) {
      await submitBooking();
    } else if (lowerCommand.includes('change') || lowerCommand.includes('edit')) {
      speak(t('voiceBooking.whatToChange', 'What would you like to change? Say "vehicle", "dates", or "information".'));
    } else if (lowerCommand.includes('vehicle')) {
      setCurrentStep(STEPS.VEHICLE);
      speak(t('voiceBooking.backToVehicle', 'Let\'s choose a different vehicle. What are you looking for?'));
    } else if (lowerCommand.includes('date')) {
      setCurrentStep(STEPS.DATES);
      speak(t('voiceBooking.backToDates', 'When would you like to pick up the vehicle?'));
    } else if (lowerCommand.includes('information') || lowerCommand.includes('info')) {
      setCurrentStep(STEPS.CUSTOMER);
      speak(t('voiceBooking.backToInfo', 'What is your first and last name?'));
    } else {
      speak(t('voiceBooking.reviewHelp', 'Say "confirm" to complete your booking, or say "change" to modify something.'));
    }
  };

  /**
   * Speak the booking summary
   */
  const speakBookingSummary = () => {
    const { vehicle, pickupDate, returnDate, customer } = bookingData;
    const days = Math.ceil((returnDate - pickupDate) / (1000 * 60 * 60 * 24));
    const totalCost = days * (vehicle.dailyRate || vehicle.DailyRate || 0);

    const summary = t('voiceBooking.summary',
      `Here's your booking summary: 
      Vehicle: {{make}} {{model}}. 
      Pickup: {{pickup}}. 
      Return: {{return}}. 
      Duration: {{days}} days. 
      Customer: {{name}}. 
      Total cost: {{total}}. 
      Say "confirm" to complete your booking, or say "change" to modify something.`,
      {
        make: vehicle.make || vehicle.Make,
        model: vehicle.model || vehicle.Model,
        pickup: pickupDate.toLocaleDateString(),
        return: returnDate.toLocaleDateString(),
        days,
        name: `${customer.firstName} ${customer.lastName}`,
        total: formatPrice(totalCost)
      }
    );

    speak(summary);
  };

  /**
   * Submit the booking
   */
  const submitBooking = async () => {
    speak(t('voiceBooking.processing', 'Processing your booking...'));

    try {
      const response = await fetch('/api/bookings', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          vehicleId: bookingData.vehicle.id || bookingData.vehicle.vehicleId,
          pickupDate: bookingData.pickupDate.toISOString(),
          returnDate: bookingData.returnDate.toISOString(),
          customer: bookingData.customer,
          extras: bookingData.extras,
          companyId
        }),
      });

      if (!response.ok) throw new Error('Booking failed');

      const result = await response.json();
      
      setCurrentStep(STEPS.COMPLETE);
      speak(
        t('voiceBooking.success',
          'Booking completed successfully! Your confirmation number is {{confirmationNumber}}. You will receive an email confirmation shortly.',
          { confirmationNumber: result.confirmationNumber || 'pending' }
        )
      );

      if (onBookingComplete) {
        onBookingComplete(result);
      }
    } catch (error) {
      console.error('Booking submission error:', error);
      speak(t('voiceBooking.bookingError', 'Sorry, there was an error completing your booking. Please try again or contact support.'));
    }
  };

  /**
   * Manual input handlers for fallback
   */
  const handleManualVehicleSelect = (vehicle) => {
    setBookingData(prev => ({ ...prev, vehicle }));
    setCurrentStep(STEPS.DATES);
  };

  const handleManualDateChange = (field, value) => {
    setBookingData(prev => ({ ...prev, [field]: new Date(value) }));
  };

  const handleManualCustomerChange = (field, value) => {
    setBookingData(prev => ({
      ...prev,
      customer: { ...prev.customer, [field]: value }
    }));
  };

  /**
   * Render current step
   */
  const renderStep = () => {
    switch (currentStep) {
      case STEPS.VEHICLE:
        return (
          <div className="voice-step">
            <div className="step-header">
              <Calendar className="w-8 h-8 text-indigo-600" />
              <h3>{t('voiceBooking.stepVehicle', 'Step 1: Choose Your Vehicle')}</h3>
            </div>
            <p className="step-instruction">
              {t('voiceBooking.vehicleInstruction', 'Describe what type of vehicle you need. For example: "I need a family car for 5 people" or "I want a luxury sedan".')}
            </p>
            
            {/* Manual fallback */}
            <div className="manual-selection">
              <p className="manual-label">{t('voiceBooking.orSelectManually', 'Or select manually:')}</p>
              <select 
                onChange={(e) => {
                  const vehicle = availableVehicles.find(v => 
                    (v.id || v.vehicleId) === e.target.value
                  );
                  if (vehicle) handleManualVehicleSelect(vehicle);
                }}
                className="manual-select"
              >
                <option value="">{t('voiceBooking.selectVehicle', 'Select a vehicle...')}</option>
                {availableVehicles.map(v => (
                  <option key={v.id || v.vehicleId} value={v.id || v.vehicleId}>
                    {(v.make || v.Make)} {(v.model || v.Model)} - {formatPrice(v.dailyRate || v.DailyRate)}/day
                  </option>
                ))}
              </select>
            </div>
          </div>
        );

      case STEPS.DATES:
        return (
          <div className="voice-step">
            <div className="step-header">
              <Calendar className="w-8 h-8 text-indigo-600" />
              <h3>{t('voiceBooking.stepDates', 'Step 2: Choose Your Dates')}</h3>
            </div>
            {bookingData.vehicle && (
              <div className="selected-vehicle">
                <p><strong>{t('voiceBooking.selectedVehicle', 'Selected Vehicle:')}</strong></p>
                <p>{(bookingData.vehicle.make || bookingData.vehicle.Make)} {(bookingData.vehicle.model || bookingData.vehicle.Model)}</p>
              </div>
            )}
            <p className="step-instruction">
              {!bookingData.pickupDate 
                ? t('voiceBooking.pickupInstruction', 'When would you like to pick up the vehicle? Say "tomorrow", "next Monday", or a specific date.')
                : t('voiceBooking.returnInstruction', 'When would you like to return it? You can say "3 days later" or a specific date.')
              }
            </p>
            
            {/* Manual fallback */}
            <div className="manual-selection">
              <label className="manual-label">
                {t('voiceBooking.pickupDate', 'Pickup Date:')}
                <input 
                  type="date" 
                  value={bookingData.pickupDate ? bookingData.pickupDate.toISOString().split('T')[0] : ''}
                  onChange={(e) => handleManualDateChange('pickupDate', e.target.value)}
                  className="manual-input"
                  min={new Date().toISOString().split('T')[0]}
                />
              </label>
              <label className="manual-label">
                {t('voiceBooking.returnDate', 'Return Date:')}
                <input 
                  type="date" 
                  value={bookingData.returnDate ? bookingData.returnDate.toISOString().split('T')[0] : ''}
                  onChange={(e) => handleManualDateChange('returnDate', e.target.value)}
                  className="manual-input"
                  min={bookingData.pickupDate ? bookingData.pickupDate.toISOString().split('T')[0] : new Date().toISOString().split('T')[0]}
                />
              </label>
              {bookingData.pickupDate && bookingData.returnDate && (
                <button 
                  onClick={() => setCurrentStep(STEPS.CUSTOMER)}
                  className="manual-button"
                >
                  {t('voiceBooking.continue', 'Continue')}
                </button>
              )}
            </div>
          </div>
        );

      case STEPS.CUSTOMER:
        return (
          <div className="voice-step">
            <div className="step-header">
              <User className="w-8 h-8 text-indigo-600" />
              <h3>{t('voiceBooking.stepCustomer', 'Step 3: Your Information')}</h3>
            </div>
            <p className="step-instruction">
              {!bookingData.customer.firstName
                ? t('voiceBooking.nameInstruction', 'Please tell me your first and last name.')
                : !bookingData.customer.email
                ? t('voiceBooking.emailInstruction', 'What is your email address?')
                : !bookingData.customer.phone
                ? t('voiceBooking.phoneInstruction', 'What is your phone number?')
                : t('voiceBooking.licenseInstruction', 'What is your driver\'s license number?')
              }
            </p>
            
            {/* Manual fallback */}
            <div className="manual-selection">
              <label className="manual-label">
                {t('voiceBooking.firstName', 'First Name:')}
                <input 
                  type="text"
                  value={bookingData.customer.firstName}
                  onChange={(e) => handleManualCustomerChange('firstName', e.target.value)}
                  className="manual-input"
                />
              </label>
              <label className="manual-label">
                {t('voiceBooking.lastName', 'Last Name:')}
                <input 
                  type="text"
                  value={bookingData.customer.lastName}
                  onChange={(e) => handleManualCustomerChange('lastName', e.target.value)}
                  className="manual-input"
                />
              </label>
              <label className="manual-label">
                {t('voiceBooking.email', 'Email:')}
                <input 
                  type="email"
                  value={bookingData.customer.email}
                  onChange={(e) => handleManualCustomerChange('email', e.target.value)}
                  className="manual-input"
                />
              </label>
              <label className="manual-label">
                {t('voiceBooking.phone', 'Phone:')}
                <input 
                  type="tel"
                  value={bookingData.customer.phone}
                  onChange={(e) => handleManualCustomerChange('phone', e.target.value)}
                  className="manual-input"
                />
              </label>
              <label className="manual-label">
                {t('voiceBooking.license', 'Driver\'s License:')}
                <input 
                  type="text"
                  value={bookingData.customer.licenseNumber}
                  onChange={(e) => handleManualCustomerChange('licenseNumber', e.target.value)}
                  className="manual-input"
                />
              </label>
              {bookingData.customer.firstName && bookingData.customer.lastName && 
               bookingData.customer.email && bookingData.customer.phone && 
               bookingData.customer.licenseNumber && (
                <button 
                  onClick={() => {
                    setCurrentStep(STEPS.REVIEW);
                    speakBookingSummary();
                  }}
                  className="manual-button"
                >
                  {t('voiceBooking.reviewBooking', 'Review Booking')}
                </button>
              )}
            </div>
          </div>
        );

      case STEPS.REVIEW:
        const days = bookingData.pickupDate && bookingData.returnDate 
          ? Math.ceil((bookingData.returnDate - bookingData.pickupDate) / (1000 * 60 * 60 * 24))
          : 0;
        const dailyRate = bookingData.vehicle?.dailyRate || bookingData.vehicle?.DailyRate || 0;
        const totalCost = days * dailyRate;

        return (
          <div className="voice-step">
            <div className="step-header">
              <CheckCircle className="w-8 h-8 text-green-600" />
              <h3>{t('voiceBooking.stepReview', 'Step 4: Review & Confirm')}</h3>
            </div>
            
            <div className="booking-summary">
              <div className="summary-section">
                <h4>{t('voiceBooking.vehicle', 'Vehicle')}</h4>
                <p>{(bookingData.vehicle?.make || bookingData.vehicle?.Make)} {(bookingData.vehicle?.model || bookingData.vehicle?.Model)}</p>
                <p className="summary-detail">{formatPrice(dailyRate)}/day</p>
              </div>

              <div className="summary-section">
                <h4>{t('voiceBooking.dates', 'Dates')}</h4>
                <p>{t('voiceBooking.pickup', 'Pickup')}: {bookingData.pickupDate?.toLocaleDateString()}</p>
                <p>{t('voiceBooking.return', 'Return')}: {bookingData.returnDate?.toLocaleDateString()}</p>
                <p className="summary-detail">{days} {t('voiceBooking.days', 'days')}</p>
              </div>

              <div className="summary-section">
                <h4>{t('voiceBooking.customerInfo', 'Customer Information')}</h4>
                <p>{bookingData.customer.firstName} {bookingData.customer.lastName}</p>
                <p>{bookingData.customer.email}</p>
                <p>{bookingData.customer.phone}</p>
                <p className="summary-detail">{t('voiceBooking.licenseNum', 'License')}: {bookingData.customer.licenseNumber}</p>
              </div>

              <div className="summary-total">
                <h4>{t('voiceBooking.totalCost', 'Total Cost')}</h4>
                <p className="total-price">{formatPrice(totalCost)}</p>
              </div>
            </div>

            <p className="step-instruction">
              {t('voiceBooking.confirmInstruction', 'Say "confirm" to complete your booking, or "change" to modify any details.')}
            </p>

            <div className="manual-selection">
              <button onClick={submitBooking} className="confirm-button">
                <CreditCard className="w-5 h-5" />
                {t('voiceBooking.confirmBooking', 'Confirm Booking')}
              </button>
              <button onClick={() => setCurrentStep(STEPS.VEHICLE)} className="change-button">
                {t('voiceBooking.startOver', 'Start Over')}
              </button>
            </div>
          </div>
        );

      case STEPS.COMPLETE:
        return (
          <div className="voice-step">
            <div className="step-header">
              <CheckCircle className="w-12 h-12 text-green-600" />
              <h3>{t('voiceBooking.bookingComplete', 'Booking Complete!')}</h3>
            </div>
            <div className="success-message">
              <p>{t('voiceBooking.thankYou', 'Thank you for your booking!')}</p>
              <p>{t('voiceBooking.confirmationSent', 'A confirmation email has been sent to {{email}}', 
                { email: bookingData.customer.email })}</p>
            </div>
          </div>
        );

      default:
        return null;
    }
  };

  return (
    <div className="voice-booking-container">
      {/* Voice Controls */}
      <div className="voice-controls">
        <button
          className={`voice-button ${isListening ? 'listening' : ''}`}
          onClick={toggleListening}
          disabled={currentStep === STEPS.COMPLETE}
        >
          <Mic className="w-6 h-6" />
          <span>
            {isListening 
              ? t('voiceBooking.listening', 'Listening...') 
              : t('voiceBooking.clickToSpeak', 'Click to Speak')}
          </span>
        </button>

        {transcript && (
          <div className="transcript-display">
            <p className="transcript-label">{t('voiceBooking.youSaid', 'You said:')}</p>
            <p className="transcript-text">"{transcript}"</p>
          </div>
        )}

        {isSpeaking && (
          <div className="speaking-indicator">
            <Volume2 className="w-5 h-5 animate-pulse" />
            <span>{t('voiceBooking.speaking', 'Speaking...')}</span>
          </div>
        )}

        {/* Voice Selection */}
        {availableVoices.length > 0 && (
          <div className="voice-selector">
            <label>{t('voiceBooking.assistantVoice', 'Assistant Voice:')}</label>
            <select
              value={selectedVoice?.name || ''}
              onChange={(e) => {
                const voice = availableVoices.find(v => v.name === e.target.value);
                setSelectedVoice(voice);
              }}
            >
              {availableVoices
                .filter(v => v.lang.startsWith(i18n.language.split('-')[0]) || v.lang.startsWith('en'))
                .map(voice => (
                  <option key={voice.name} value={voice.name}>
                    {voice.name}
                  </option>
                ))
              }
            </select>
          </div>
        )}
      </div>

      {/* Progress Indicator */}
      <div className="progress-steps">
        <div className={`progress-step ${currentStep === STEPS.VEHICLE ? 'active' : ''} ${[STEPS.DATES, STEPS.CUSTOMER, STEPS.REVIEW, STEPS.COMPLETE].includes(currentStep) ? 'completed' : ''}`}>
          <div className="step-number">1</div>
          <div className="step-label">{t('voiceBooking.vehicle', 'Vehicle')}</div>
        </div>
        <div className={`progress-step ${currentStep === STEPS.DATES ? 'active' : ''} ${[STEPS.CUSTOMER, STEPS.REVIEW, STEPS.COMPLETE].includes(currentStep) ? 'completed' : ''}`}>
          <div className="step-number">2</div>
          <div className="step-label">{t('voiceBooking.dates', 'Dates')}</div>
        </div>
        <div className={`progress-step ${currentStep === STEPS.CUSTOMER ? 'active' : ''} ${[STEPS.REVIEW, STEPS.COMPLETE].includes(currentStep) ? 'completed' : ''}`}>
          <div className="step-number">3</div>
          <div className="step-label">{t('voiceBooking.info', 'Info')}</div>
        </div>
        <div className={`progress-step ${currentStep === STEPS.REVIEW ? 'active' : ''} ${currentStep === STEPS.COMPLETE ? 'completed' : ''}`}>
          <div className="step-number">4</div>
          <div className="step-label">{t('voiceBooking.review', 'Review')}</div>
        </div>
      </div>

      {/* Current Step Content */}
      <div className="step-content">
        {renderStep()}
      </div>
    </div>
  );
};

export default VoiceBooking;
