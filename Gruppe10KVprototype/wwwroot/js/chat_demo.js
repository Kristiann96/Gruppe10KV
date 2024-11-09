class ChatHandler {
    constructor() {
        console.log('ChatHandler constructor called');

        this.context = {
            issue: "Jeg vil rapportere en feil i merkingen av turstien til Preikestolen på kartet deres.",
            details: "Den nye alternative ruten som ble etablert i fjor sommer er ikke tegnet inn, og den gamle ruten som nå er stengt vises fortsatt som hovedrute.",
            location: "Preikestolen i Rogaland",
            date: "3. november 2023",
            importance: "Dette er en av Norges mest besøkte turistattraksjoner, og det er viktig at turister og turgåere har korrekt informasjon om hvilken rute de skal følge."
        };

        this.chatMessages = document.getElementById('chatMessages');
        this.messageInput = document.getElementById('messageInput');
        this.chatForm = document.getElementById('chatForm');
        this.isTyping = false;

        this.handleSubmit = this.handleSubmit.bind(this);
        if (this.chatForm) {
            this.chatForm.addEventListener('submit', this.handleSubmit);
            console.log('Submit handler attached');
        } else {
            console.error('Chat form not found');
        }
    }

    async handleSubmit(event) {
        event.preventDefault();
        const messageText = this.messageInput.value.trim();

        if (!messageText) return;

        this.addMessage(messageText, 'outgoing');
        this.messageInput.value = '';

        this.showTypingIndicator();
        await new Promise(resolve => setTimeout(resolve, 1000));

        const response = this.getMockResponse(messageText);
        this.removeTypingIndicator();
        this.addMessage(response, 'incoming');
    }

    getMockResponse(message) {
        const lowerMessage = message.toLowerCase();

        // Greeting patterns
        if (lowerMessage.includes('hei') || lowerMessage.includes('hallo')) {
            return `Hei! ${this.context.issue}`;
        }

        // Location questions
        if (lowerMessage.includes('hvor')) {
            return `Dette gjelder ${this.context.location}. Den nye alternative ruten ble laget for å håndtere den økte turisttrafikken og gi en bedre og sikrere turopplevelse, men dette reflekteres ikke i kartet deres.`;
        }

        // Time/date questions
        if (lowerMessage.includes('når')) {
            return `Jeg oppdaget dette ${this.context.date} da jeg skulle planlegge en tur. Den nye ruten ble åpnet i fjor sommer, men kartet viser fortsatt bare den gamle ruten som nå er stengt.`;
        }

        // What/explanation questions
        if (lowerMessage.includes('hva') || lowerMessage.includes('forklar') || lowerMessage.includes('problem')) {
            return `${this.context.issue} ${this.context.details} Dette kan være forvirrende og potensielt farlig for besøkende som stoler på kartinformasjonen.`;
        }

        // Status/update questions
        if (lowerMessage.includes('status') || lowerMessage.includes('oppdater')) {
            return `Den gamle ruten er nå stengt på grunn av sikkerhetshensyn og slitasje, mens den nye alternative ruten er godt merket i terrenget. Kartet bør oppdateres for å reflektere disse endringene så snart som mulig.`;
        }

        // Details about the trail
        if (lowerMessage.includes('sti') || lowerMessage.includes('rute') || lowerMessage.includes('vei')) {
            return `Den nye ruten er bedre tilrettelagt med tydeligere merking, flere hvileplasser og bedre sikring på utsatte partier. Den gamle ruten som vises i kartet er nå stengt på grunn av erosjon og sikkerhetshensyn.`;
        }

        // Safety concerns
        if (lowerMessage.includes('sikkerhet') || lowerMessage.includes('farlig') || lowerMessage.includes('risiko')) {
            return `Det er viktig at kartet oppdateres fordi feil informasjon kan føre til at folk følger den gamle, stengte ruten. Dette kan være farlig, spesielt i dårlig vær eller for uerfarne turgåere.`;
        }

        // Thanks
        if (lowerMessage.includes('takk')) {
            return 'Bare hyggelig! Det er viktig at populære turområder som Preikestolen har oppdatert og presis kartinformasjon for alles sikkerhet.';
        }

        // Questions about importance
        if (lowerMessage.includes('hvorfor') || lowerMessage.includes('viktig')) {
            return this.context.importance + ' Feil kartinformasjon kan føre til misforståelser og potensielt farlige situasjoner.';
        }

        // Default response
        return `${this.context.issue} Dette er en mye brukt tursti, og det er viktig at kartene er oppdaterte og nøyaktige for sikkerheten til alle besøkende.`;
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
});