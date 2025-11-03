# 🧩 Project Ambev

---

## 🗂️ Arquitetura do Projeto

### **Adapters**
Adaptadores responsáveis pela comunicação entre dados e interface.

#### **Infrastructure**
ORM do projeto responsável pela integração com a base de dados.

**Otimizações utilizadas:**
- Migrations  
- Data First com geração automática de tabelas utilizando **Entity Framework Core**

#### **WebApi**
Extensão de conexão do projeto contendo **Controllers**, **Request Objects**, **Response Objects** e **Validation Requests**.

**Otimizações utilizadas:**
- Autenticação **JWT** nativa da Microsoft  
- Segurança contra **SQL Injection**  
- Mapeamento dinâmico para evitar redundância de código  
- **Docker** configurado para hospedagem do serviço  
- Serviço **Kafka** configurado para mensageria  

---

### **Core**
Coração da aplicação, onde toda a regra de negócio está centralizada.

#### **Application**
Contém **Commands**, **Handlers**, **Fakers** e **Configs**.

**Otimizações utilizadas:**
- Fakers para gerar Mocks usando o conceito do **Bogus** no .NET  
- Redução de redundância de código  
- Implementação do padrão **CQRS** (separação entre leitura e escrita) para maior performance  

#### **Domain**
Estrutura de domínio do projeto, responsável pela concentração e modelagem dos objetos de negócio.

---

### **Crosscutting**
Responsável por garantir baixo acoplamento e alta coesão entre os módulos do sistema.

#### **Common**
Estrutura comum da aplicação com métodos e regras compartilhadas, seguindo o conceito do **Adapter Pattern**.

#### **IoC (Inversion of Control)**
Modularização do projeto com injeção de dependência apenas do que é estritamente necessário, respeitando os princípios da **arquitetura limpa**.

---

### **Tests**
Testes unitários com cobertura total (**100%**) do código.

**Otimizações utilizadas:**
- Geração de Mocks com **Bogus** para evitar redundância  
- Mocks criados sempre por **interface** (nunca por classe concreta)  
- Utilização do **XUnit** para comparação e validação de objetos  

---

## ⚙️ **Principais Práticas Utilizadas**

- **Data First** para geração de tabelas via Entity Framework Core com Migrations  
- **Docker** configurado para containerização da aplicação  
- **Pipeline CI/CD** configurado (**Azure DevOps**)  
- **Apache Kafka** configurado para mensageria  
- **Domain-Driven Design (DDD)** como estrutura base  
- **Clean Code** aplicado em toda a solução  
- **S.O.L.I.D** – nenhuma classe depende diretamente de outra, apenas de interfaces  
- **CQRS** – separação clara entre leitura e escrita  
- **Lazy Loading** e chamadas **assíncronas** configuradas no Entity Framework  
- **Recursividade** aplicada em Business e Common Layers  
- **Mocks** com **Faker** e **Bogus** para evitar redundância  
- **Testes unitários** com ampla cobertura  
- **Autenticação JWT** utilizando biblioteca oficial da Microsoft  
- **Validações (Validators)** para garantir consistência e restrições de dados  

---

---
> "Eu disse essas coisas para que em mim vocês tenham paz. Neste mundo vocês terão aflições; contudo, tenham ânimo! Eu venci o mundo".
- João 16
---
> Desenvolvido com Rider 💻 por Eduardo Amorim
