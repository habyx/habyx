import React, { useState, useEffect } from 'react';
import './Chat.css';

const Chat = () => {
  const [messages, setMessages] = useState<string[]>([]);
  const [input, setInput] = useState('');

  const handleSendMessage = (e: React.FormEvent) => {
    e.preventDefault();
    if (input.trim()) {
      setMessages([...messages, input]);
      setInput('');
    }
  };

  useEffect(() => {
    // Placeholder for loading initial messages
    // You can fetch messages from an API here
  }, []);

  return (
    <div className="chat-container">
      <div className="messages">
        {messages.map((msg, index) => (
          <div key={index} className="message">
            {msg}
          </div>
        ))}
      </div>
      <form onSubmit={handleSendMessage}>
        <input
          type="text"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Type a message..."
          required
        />
        <button type="submit">Send</button>
      </form>
    </div>
  );
};

export default Chat;