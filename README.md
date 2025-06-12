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

## 💡 Usage Notes

- Item names are auto-fetched from `cq_itemtype` table.
- Only numeric values can be entered for price/amount fields.
- Version field is locked (not editable).
- To add new item, use the "Add Item" button,select shop type, search, then double-click item.
- **All changes are applied instantly; be sure to back up your files.**
