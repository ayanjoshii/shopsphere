# Resume Updates + Interview Prep: S&P Global Market Intelligence (Apprentice Developer)

> Rule: add C#/.NET to your resume only after you have run ShopSphere yourself and understood the code. Interviewers will ask about it.

## 1. Professional Summary (replace current one)

Curious and improvement-driven B.Tech Computer Science student with hands-on experience in C#/.NET, Java, Python and JavaScript, a software development internship, and Microsoft Azure certifications (AZ-204, AZ-900). Builds data-driven, customer-focused applications, from REST APIs and cloud-based backends to AI healthcare systems, and enjoys working in agile teams to turn requirements into reliable, well-tested software.

## 2. Technical Skills section (updated)

- **Languages:** C#, Java, Python, JavaScript, C, C++, SQL, HTML, CSS
- **Backend & APIs:** .NET 8, ASP.NET Core Web API, REST APIs, Entity Framework Core, LINQ, JWT Authentication
- **Databases:** Microsoft SQL Server (T-SQL, joins, indexing, transactions)
- **Cloud & Tools:** Microsoft Azure (AZ-204, AZ-900), Git, GitHub, Swagger/Postman, Visual Studio, VS Code
- **Data & Machine Learning:** TensorFlow, Keras, Scikit-learn, OpenCV, MediaPipe, NumPy, Pandas
- **Practices:** Agile/Scrum, OOP, SOLID principles, Repository pattern, Dependency Injection, Layered architecture, Code Reviews, Debugging & Testing, DSA, DBMS

## 3. New Project entry (put it FIRST in Projects)

**ShopSphere: E-Commerce Backend** | C# | .NET 8 | ASP.NET Core | EF Core | SQL Server | JWT | Swagger | GitHub: <your link>
- Built a RESTful e-commerce backend in **ASP.NET Core 8** using a layered architecture (Controllers, Services, Repositories) with the **Repository pattern** and **Dependency Injection**.
- Designed a normalized **SQL Server** schema with **EF Core (Code First)**: Users, Products, Categories, Cart, Orders, OrderItems with indexes, foreign keys and unique constraints.
- Implemented **JWT authentication and role-based authorization** (Admin/Customer) with BCrypt password hashing.
- Developed product search with filtering, sorting and **pagination** using LINQ; built cart and checkout that runs in a **database transaction** to keep stock and orders consistent.
- Added global exception-handling middleware, DTO-based validation and **Swagger** API documentation.

## 4. Cover letter / application answer (short)

I am a final-year B.Tech Computer Science student who enjoys turning business requirements into reliable software. I recently built ShopSphere, a C#/.NET 8 Web API with SQL Server, JWT security and transactional order processing, and I hold Azure Developer Associate (AZ-204) and Azure Fundamentals certifications, so cloud-based application development is something I have already worked with. The syndicated loan and financial data domain at S&P Global Market Intelligence interests me because accuracy and data quality matter so much there. I learn quickly, work well in agile teams and would like to grow under the guidance of senior developers.

## 5. AWS gap: say this in the interview

"My cloud experience is on Azure (AZ-204, AZ-900). The concepts map directly, so I can pick up AWS quickly."

| Azure (you know) | AWS equivalent |
|---|---|
| Virtual Machines | EC2 |
| Blob Storage | S3 |
| Azure Functions | Lambda |
| Azure SQL Database | RDS |
| App Service | Elastic Beanstalk |
| Azure AD / Entra ID | IAM / Cognito |
| Azure Monitor | CloudWatch |
| Azure DevOps / Pipelines | CodePipeline / CodeBuild |
| Service Bus / Queue Storage | SQS / SNS |

Optional quick win: AWS Certified Cloud Practitioner (2-3 weeks of study).

## 6. Interview prep checklist

**C# / .NET**
- 4 pillars of OOP with examples from ShopSphere (e.g. interfaces + DI = abstraction/polymorphism)
- class vs struct, interface vs abstract class, value vs reference types
- `async/await`, `Task`, why `await` does not block a thread
- LINQ: `Where`, `Select`, `GroupBy`, `Join`; deferred execution
- Dependency Injection lifetimes: Singleton, Scoped, Transient (why DbContext is Scoped)
- Middleware pipeline, what `UseAuthentication` vs `UseAuthorization` does
- `IEnumerable` vs `IQueryable`
- Exceptions: try/catch/finally, custom exceptions (AppException)

**SQL (MS SQL Server)**
- INNER / LEFT / RIGHT joins, GROUP BY + HAVING, subqueries
- Primary key vs foreign key, indexes (clustered vs non-clustered), why you indexed `Product.Name`
- Normalization (1NF to 3NF), transactions and ACID (explain checkout)
- Practice: top 5 products by revenue, customers with no orders, orders per month

**Web / REST**
- HTTP methods, status codes (200, 201, 204, 400, 401, 403, 404, 409, 500)
- JWT structure (header.payload.signature), how auth flow works, why hash passwords
- REST principles, DTOs vs entities, pagination

**Design / Agile / Git**
- Repository pattern, layered architecture, SOLID (be able to give one example each)
- Scrum ceremonies: daily stand-up, sprint planning, review, retrospective
- Git: branch, merge vs rebase, pull request, resolving conflicts

**Project questions to be ready for**
1. Why did you use the Repository pattern? What are its downsides?
2. How does checkout stay consistent if two users buy the last item? (transaction, concurrency token/rowversion is the next improvement)
3. How would you add Razorpay payments and caching?
4. How would you deploy this on Azure? (App Service + Azure SQL + Key Vault for secrets)

## 7. 2-week plan

| Days | Task |
|---|---|
| 1-2 | Install .NET 8 SDK + SQL Server, run ShopSphere, test every endpoint in Swagger |
| 3-5 | Read every file, understand each line, add 1 feature yourself (e.g. wishlist or reviews) |
| 6-7 | Write xUnit tests for ProductService and AuthService |
| 8-9 | Push to GitHub with this README, add screenshots of Swagger |
| 10 | Deploy to Azure App Service + Azure SQL (uses your AZ-204 knowledge) |
| 11-14 | Revise SQL joins, C# basics, mock interviews; update resume and apply |

## 8. Final checklist before applying
- [ ] ShopSphere on GitHub with README
- [ ] Resume summary, skills and project updated (sections 1-3)
- [ ] Resume PDF named `Ayan_Joshi_Resume.pdf`
- [ ] LinkedIn skills updated with C#, .NET, SQL Server
- [ ] Cover letter answer customized (section 4)
