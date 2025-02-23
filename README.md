# **PharmaEase – ASP.NET MVC Web Application**  

## **Overview**  
**PharmaEase** is an **ASP.NET MVC**-based **e-commerce platform** for pharmaceutical products. It allows users to **search, filter, add to cart, checkout, and provide feedback** on medicines. The platform uses **SignalR** for real-time updates, **Microsoft SQL Server** with **Dapper** for database operations, and supports **user authentication with roles (Admin & User)**.  

## **Features**  

### 🔍 **Search & Filtering**  
- **jQuery-based search functionality** for quick medicine lookup.  
- **Advanced filtering options** to find medicines based on category, price.  

### 🛒 **E-Commerce Functionalities**  
- **Add to Cart & Checkout** – Users can **add medicines to the cart** and proceed with a seamless **checkout process**.  
- **User Authentication** – Secure **login & registration system** with **Admin & User roles**.  
- **Session Management** – Ensures user session persistence during shopping.  

### 📢 **Real-Time Notifications (SignalR)**  
- **Admin notifications** when a new medicine is added.   

### 📊 **Feedback System**  
- Users can **feedback** on medicines to help others make informed choices.  

### ⚙ **Admin Features**  
- **Manage Medicines** – Add, update, or remove medicines from the inventory.  
- **View & Manage Orders** – Track and process user orders efficiently.  

## **Technology Stack**  
- **Backend:** ASP.NET MVC, C#  
- **Frontend:** jQuery, HTML, CSS, Bootstrap  
- **Database:** Microsoft SQL Server, Dapper ORM  
- **Real-time Communication:** SignalR  
- **Authentication:** ASP.NET Identity (Admin & User Roles)  

## **Setup Instructions**  

### **1. Clone the Repository**  
```bash
git clone https://github.com/your-repo/pharmaease.git
cd pharmaease
```

### **2. Database Setup**  
- Install **Microsoft SQL Server**.  
- Update the **connection string** in `appsettings.json`.  
- Run the following command to apply migrations:  
  ```bash
  update-database
  ```

### **3. Run the Application**  
- Open the project in **Visual Studio**.  
- Set **RealProject.Web** as the startup project.  
- Click **Run** or execute:  
  ```bash
  dotnet run
  ```


🚀 **PharmaEase – Your One-Stop Solution for Easy Medicine Shopping!**
