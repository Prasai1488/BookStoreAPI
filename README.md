# 📚 BookStore - Full Stack Web App

A private library store going digital. This system allows users to browse and purchase books online, with advanced filtering, search, and sorting features. Registered members can whitelist books, add to cart, place in-store pickup orders, receive claim codes via email, and review only purchased books. The platform includes real-time features, tiered discounts, and a full admin dashboard for inventory, discounts, and announcements.

---

## 🔗 Live Demo

- Frontend: [book-store-ui-rust.vercel.app](https://book-store-ui-rust.vercel.app)
- Backend API: [bookstoreapi-ym8m.onrender.com](https://bookstoreapi-ym8m.onrender.com)


---

 Key Features Summary
🔍 Browse, search, filter, and sort books (genre, author, format, rating, price, etc.)

🛒 Members: Register, bookmark, add to cart, cancel orders

🧾 Receive email claim code and bill for in-store pickup

💬 Post-purchase review & rating system

🎁 Tiered discounts (5% for 5+ books, 10% after 10 orders)

🧑‍💼 Admin: Full book inventory management, timed discounts & announcements

🔔 Real-time announcement banners and order broadcasts using SignalR

---

## 👨‍💻 User Roles

- **Staff**: Staff can process the member claim code to fulfil the order
- **Registered User**: Leave reviews, place orders,Bookmark books, add to cart.
- **Admin**: Add/edit/delete books, post announcements

---

## 📝 How to Test the App

You can use the following test credentials to explore:

- **Admin**  
  Email: `prasaiprithvi4@gmail.com`  
  Password: `Admin@123`

- **Regular User**  
  Register manually with a valid email or use:
  Email: `sandeep@gmail.com`  
  Password: `test@123`

---

## 🛠️ Tech Stack

- **Frontend:** React, Redux Toolkit, TypeScript, Vite
- **Backend:** .NET Core Web API, Entity Framework Core, PostgreSQL
- **Other:** SignalR, SMTP for emails, Axios

---

## ⚙️ Setup Instructions

### Backend

```bash
cd backend
dotnet restore
dotnet run

