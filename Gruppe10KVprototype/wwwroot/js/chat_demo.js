class ChatHandler {
    constructor() {
        console.log('ChatHandler constructor called');

        // Your OpenAI API key - WARNING: Don't expose this in production!
        this.apiKey = 'your-api-key-here';

        this.messages = [{
            role: "system",
            content: "Du er en hjelpsom kundeservice-representant for Kartverket som svarer på spørsmål om kartfeil. Svar på norsk og vær hjelpsom og profesjonell."
        }];

        this.chatMessages = document.getElementById('chatMessages');
        this.messageInput = document.getElementById('messageInput');
        this.chatForm = document.getElementById('chatForm');
        this.isTyping = false;

        // Debug logging
        console.log('Looking for elements:', {
            chatMessages: !!this.chatMessages,
            messageInput: !!this.messageInput,
            chatForm: !!this.chatForm,
        });

        this.handleSubmit = this.handleSubmit.bind(this);
        if (this.chatForm) {
            this.chatForm.addEventListener('submit', this.handleSubmit);
        }
    }

    async handleSubmit(event) {
        event.preventDefault();
        const messageText = this.messageInput.value.trim();

        if (!messageText) return;

        // Display user message
        this.addMessage(messageText, 'outgoing');
        this.messageInput.value = '';

        // Add to messages array
        this.messages.push({
            role: "user",
            content: messageText
        });

        this.showTypingIndicator();

        try {
            const response = await fetch('https://api.openai.com/v1/chat/completions', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${this.apiKey}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    model: 'gpt-3.5-turbo',
                    messages: this.messages,
                    temperature: 0.7
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            const assistantMessage = data.choices[0].message.content;

            // Add assistant's response to conversation history
            this.messages.push({
                role: "assistant",
                content: assistantMessage
            });

            this.removeTypingIndicator();
            this.addMessage(assistantMessage, 'incoming');

        } catch (error) {
            console.error('Error:', error);
            this.removeTypingIndicator();
            this.addMessage('Beklager, jeg kunne ikke behandle meldingen din. Vennligst prøv igjen.', 'incoming');
        }
    }

    addMessage(text, type) {
        const messageDiv = document.createElement('div');
        messageDiv.classList.add('message', `message-${type}`);

        messageDiv.innerHTML = `
            ${this.formatMessage(text)}
            <div class="message-time">${this.getCurrentTime()}</div>
        `;

        this.chatMessages.appendChild(messageDiv);
        this.scrollToBottom();
    }

    formatMessage(text) {
        // Convert URLs to clickable links
        return text.replace(/(https?:\/\/[^\s]+)/g, '<a href="$1" target="_blank">$1</a>');
    }

    getCurrentTime() {
        return new Date().toLocaleTimeString('no-NO', {
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    scrollToBottom() {
        this.chatMessages.scrollTop = this.chatMessages.scrollHeight;
    }

    showTypingIndicator() {
        if (!this.isTyping) {
            this.isTyping = true;
            const typingDiv = document.createElement('div');
            typingDiv.classList.add('typing-indicator');
            typingDiv.id = 'typingIndicator';
            typingDiv.textContent = 'Skriver...';
            this.chatMessages.appendChild(typingDiv);
            this.scrollToBottom();
        }
    }

    removeTypingIndicator() {
        const typingIndicator = document.getElementById('typingIndicator');
        if (typingIndicator) {
            typingIndicator.remove();
        }
        this.isTyping = false;
    }
}

// Initialize chat when document is ready
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM loaded, initializing chat');
    const chat = new ChatHandler();

    // Add visibility check
    const chatContainer = document.querySelector('.chat-container');
    if (chatContainer) {
        console.log('Chat container found');
        console.log('Chat container style:', window.getComputedStyle(chatContainer));
    } else {
        console.error('Chat container not found in DOM');
    }
});