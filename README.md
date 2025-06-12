# Event Shop Editor EO

**Event Shop Editor EO** is a free and open-source Windows tool to manage the event shop (`activitynewshop.ini`) for Eudemons Online private servers.  
Easily connect to your MySQL 8 database, browse and update shop items (Astra, Honor, Plane) with instant item name lookup, editing, drag-and-drop, and more.

## 🛠 Features

- **Edit Astra, Honor, Plane shop data** visually from `activitynewshop.ini`
- **MySQL 8 compatible** (cq_itemtype item name lookup)
- **Quick item search** with live search box
- **Drag and drop rows** to reorder items
- **Add & delete items** with a simple UI
- **Bulk editing**: change price, amount, and other fields instantly
- **Sync with database**: truncate and update table with new config
- **Open source** — free for any use

## 🚀 Getting Started

1. **Download/clone** this repo.
2. **Build** with Visual Studio 2022+ (WinForms, .NET 6/7/8)
3. **Run** `EventShopEditorEO.exe`
4. **Connect** to your MySQL 8 DB (fill host, port, user, password, DB name)
5. **Select your client folder** (it will auto-detect `activitynewshop.ini`)
6. **Edit shop items** in the Shop Setting tab:
   - Astra Shop
   - Honor Shop
   - Plane Shop
7. **Drag-and-drop** or use right-click (move/top/bottom, delete)
8. **Save** to update both the INI file and database table

## 💡 Usage Notes

- Item names are auto-fetched from `cq_itemtype` table.
- Only numeric values can be entered for price/amount fields.
- Version field is locked (not editable).
- To add new item, use the "Add Item" button, search, then double-click item.
- **All changes are applied instantly; be sure to back up your files.**
