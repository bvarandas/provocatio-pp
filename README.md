# Provocatio-pp


Project proposed for a challenge.
---

The initial proposal is to consume information from the API of the website https://hacker-news.firebaseio.com/ and be able to make a considerable number of requests without risking overloading the HACK NEWS API.

Relevant architectural requirements:
* The API and background service must not be unavailable.
* The service receives 500 requests per second, with a maximum of 1% request loss.

---

The choice for this messaging architecture was made after analyzing the architectural requirements proposed in the challenge.
It is an approach similar to the architecture we use in the capital markets for trading and pre-trade and post-trade risk analysis with caching and Threads for message queuing and SignalR for the user page.

Explaining the Technology Stack and Patterns:

**Nginx** - Important for load balancing and reverse proxy in a DMZ environment. It can also manage and validate SignalR, WebSocket, and REST API connections, verifying whether the client is valid or not before reaching the backend environment. We do not perform identity controls; however, we can control it via JWT with a bearer. Important for controlling the number of requests made to the API, using the Round Robin algorithm to distribute requests among API instances, and also important for high availability and resilience to high request demand, using reverse proxy and API tonal configuration.

**Blazor** - The front-end application was built in Blazor so we could view the news listing. Tech was chosen for its ease of use and ease of integration with SignalR.

**Protobuf** -  For message compression during inter-service transport (Byte Array) - Important for message compression during transport and for better storage and sending via RabbitMQ.

**DragoFlyDB** - Important for temporarily storing outbox messages since its I/O can handle more than 2K per second. This tool will likely replace Redis in a few months. It's important to note that it accepts transactional writes. For this reason, we opted for this solution since the proposed architectural requirement has a performance characteristic.

**Docker** - To put part of the application, or the entire application, into containers. However, ideally, you should only scale part of the application.

**Kubernetes** - For deploying applications in containers. Scalability is a key part of the architecture here.

**SignalR**  - To response to the client/Blazor (Screen) - Important for asynchronous reception of hack news cache information on the screen.

**This approach also restricts each service to having its own separate responsibility, ensuring programming cohesion (code) and keeping its business logic decoupled, improving not only performance but also maintainability. It's also possible to see in the diagram that it's possible to enable and scale one part of the system horizontally and another part vertically, achieving maximum savings in infrastructure resource costs.**
---

C4 architecture model

<img width="654" height="1195" alt="image" src="https://github.com/user-attachments/assets/a1500d5f-865b-4347-84d0-fea358aa3cda" />

---

Creational Patterns used:
* Singleton - to use only one instance of each module.

Behavioral Patterns:
* Mediator 

More Patterns:
**Dependency Injection** - Injecting dependencies to use object interfaces, decoupling method calls between objects.

Some concepts from SOLID principles were also used, such as:
* **Single Responsibility Principle** - should have a single responsibility (lack of cohesion, high coupling, difficulties in implementing automated tests, difficulty in reusing code)
* **Open/Close Principle** - Entities should be open for extension, but closed for modification.
(Use abstract classes, e.g., CashFlowCommand, concrete classes InsertCashFlowCommand and UpdateCashFlowCommand)
* **Interface Segregation Principle** - Many specific interfaces are better than one general interface.
(Violation of IRepository for CashFlow and DailyConsolidated repositories. Ideally, use ICashFlowRepository, and an IDailyConsolidatedRepository for each class)
* **Dependency Inversion Principle** - Depend on abstractions and not on concrete classes.

---

App screenshots:
<img width="1680" height="1238" alt="image" src="https://github.com/user-attachments/assets/4a682c0d-2fcc-44fe-b5a9-01bb1567df01" />

---

Instructions for running



Set up Visual Studio to run Docker Compose.
<img width="1708" height="129" alt="image" src="https://github.com/user-attachments/assets/478a0244-ec6f-4ae4-bda7-e36112989056" />


Press F5 to load the application

The following containers will be uploaded.
<img width="1415" height="444" alt="image" src="https://github.com/user-attachments/assets/4b43b747-82da-4894-99aa-8c4e6dd5f42d" />



---

F5 to load the application

In Visual Studio Code, go to the \src\Challenge.Front folder.
<img width="1712" height="938" alt="image" src="https://github.com/user-attachments/assets/93720156-fbab-4cff-8037-a203f50c027c" />



Access via browser: http://localhost:4200/

---


Thank you for visiting, you are welcome to offer any suggestions for improvement.!!!



