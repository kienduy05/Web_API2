import flask
from config.db import get_connection

dashboard_bp = flask.Blueprint("dashboard_bp", __name__)

@dashboard_bp.route('/dashboard/full', methods=['GET'])
def dashboard_full():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # ===== SUMMARY =====
        cursor.execute("""
            SELECT 
                COUNT(*) as TotalOrders,
                ISNULL(SUM(OrderTotalAmount), 0) as TotalRevenue
            FROM Orders
            WHERE OrderStatus = 2
        """)
        summary_row = cursor.fetchone()

        total_orders = summary_row[0]
        total_revenue = summary_row[1]

        cursor.execute("SELECT COUNT(*) FROM Book")
        total_books = cursor.fetchone()[0]

        cursor.execute("SELECT COUNT(*) FROM Customer")
        total_customers = cursor.fetchone()[0]

        # ===== CHART ===== (PascalCase để match View)
        cursor.execute("""
            SELECT 
                MONTH(OrderCreatedDate) as Month,
                ISNULL(SUM(OrderTotalAmount),0) as Revenue
            FROM Orders
            WHERE OrderStatus = 2
            GROUP BY MONTH(OrderCreatedDate)
            ORDER BY Month
        """)
        revenue_by_month = [
            {"Month": row[0], "Revenue": float(row[1])}
            for row in cursor.fetchall()
        ]

        # ===== TOP BOOK =====
        cursor.execute("""
            SELECT TOP 5 b.BookName, SUM(od.Quantity) AS TotalSold
            FROM OrderDetail od
            JOIN Book b ON od.BookID = b.BookID
            GROUP BY b.BookName
            ORDER BY TotalSold DESC
        """)
        top_books = [
            {"BookName": row[0], "TotalSold": row[1]}
            for row in cursor.fetchall()
        ]

        # ===== RECENT ORDERS =====
        cursor.execute("""
            SELECT TOP 5 OrderID, ReceiverName, OrderTotalAmount
            FROM Orders
            ORDER BY OrderCreatedDate DESC
        """)
        recent_orders = [
            {
                "OrderID": row[0],
                "ReceiverName": row[1],
                "Total": float(row[2])
            }
            for row in cursor.fetchall()
        ]

        cursor.close()
        conn.close()

        return flask.jsonify({
            "Summary": {
                "TotalBooks": total_books,
                "TotalCustomers": total_customers,
                "TotalOrders": total_orders,
                "TotalRevenue": float(total_revenue)
            },
            "RevenueByMonth": revenue_by_month,
            "TopBooks": top_books,
            "RecentOrders": recent_orders
        })

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500