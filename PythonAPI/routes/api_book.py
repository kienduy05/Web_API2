import flask
from config.db import get_connection

book_bp = flask.Blueprint("book_bp", __name__)

# 1. GET ALL (Lấy toàn bộ sách kèm thông tin chi tiết)
@book_bp.route('/book/getall', methods=['GET'])
def get_all_book():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # Dùng JOIN để lấy tên Category, Author, Publisher cho Frontend dễ hiển thị
        query = """
                SELECT b.*, c.BookCategoryName, a.AuthorName, p.PublisherName
                FROM Book b
                         LEFT JOIN BookCategory c ON b.BookCategoryID = c.BookCategoryID
                         LEFT JOIN Author a ON b.BookAuthorID = a.AuthorID
                         LEFT JOIN Publisher p ON b.BookPublisherID = p.PublisherID \
                """
        cursor.execute(query)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()
        return flask.jsonify(results)
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 2. GET BY ID
@book_bp.route('/book/getbyid/<id>', methods=['GET'])
def get_book_by_id(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM Book WHERE BookID = ?", (id,))

        keys = [col[0] for col in cursor.description]
        row = cursor.fetchone()

        cursor.close()
        conn.close()

        if row:
            return flask.jsonify(dict(zip(keys, row)))
        return flask.jsonify({"mess": "Khong tim thay sach"}), 404
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 3. INSERT (Thêm sách mới)
@book_bp.route('/book/add', methods=['POST'])
def add_book():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
                  INSERT INTO Book (BookName, BookDescription, BookCategoryID,
                                    BookAuthorID, BookPublisherID, BookQuantity,
                                    BookPrice, BookStatus, BookImage)
                  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?) \
                  """
        params = (
            data.get("BookName"),
            data.get("BookDescription"),
            data.get("BookCategoryID"),
            data.get("BookAuthorID"),
            data.get("BookPublisherID"),
            data.get("BookQuantity"),
            data.get("BookPrice"),
            data.get("BookStatus", 1),
            data.get("BookImage")
        )

        cursor.execute(command, params)
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Them sach thanh cong"}), 201
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500
# 4. UPDATE
@book_bp.route('/book/update', methods=['PUT'])
def update_book():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
                  UPDATE Book
                  SET BookName=?, \
                      BookDescription=?, \
                      BookCategoryID=?,
                      BookAuthorID=?, \
                      BookPublisherID=?, \
                      BookQuantity=?,
                      BookPrice=?, \
                      BookStatus=?, \
                      BookImage=?
                  WHERE BookID = ? \
                  """
        params = (
            data["BookName"], data["BookDescription"], data["BookCategoryID"],
            data["BookAuthorID"], data["BookPublisherID"], data["BookQuantity"],
            data["BookPrice"], data["BookStatus"], data["BookImage"],
            data["BookID"]
        )

        cursor.execute(command, params)
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Cap nhat sach thanh cong"})
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500
# 5. DELETE
@book_bp.route('/book/delete/<id>', methods=['DELETE'])
def delete_book(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()
        cursor.execute("DELETE FROM Book WHERE BookID = ?", (id,))
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Xoa sach thanh cong"})
    except Exception as e:

        return flask.jsonify({"mess": "Khong the xoa vi sach dang co trong don hang", "error": str(e)}), 400
@book_bp.route('/book/search', methods=['GET'])
def search_book():
    try:
        keyword = flask.request.args.get("keyword", "")

        conn = get_connection()
        cursor = conn.cursor()

        query = """
                SELECT b.*, c.BookCategoryName, a.AuthorName, p.PublisherName
                FROM Book b
                LEFT JOIN BookCategory c ON b.BookCategoryID = c.BookCategoryID
                LEFT JOIN Author a ON b.BookAuthorID = a.AuthorID
                LEFT JOIN Publisher p ON b.BookPublisherID = p.PublisherID
                WHERE b.BookName LIKE ?
                """

        cursor.execute(query, (f"%{keyword}%",))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500
@book_bp.route('/book/getall-with-meta', methods=['GET'])
def get_all_book_with_meta():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # Lấy sách kèm JOIN
        cursor.execute("""
            SELECT b.*, c.BookCategoryName, a.AuthorName, p.PublisherName
            FROM Book b
            LEFT JOIN BookCategory c ON b.BookCategoryID = c.BookCategoryID
            LEFT JOIN Author a ON b.BookAuthorID = a.AuthorID
            LEFT JOIN Publisher p ON b.BookPublisherID = p.PublisherID
        """)
        keys = [col[0] for col in cursor.description]
        books = [dict(zip(keys, row)) for row in cursor.fetchall()]

        # Lấy danh mục
        cursor.execute("SELECT * FROM BookCategory")
        keys = [col[0] for col in cursor.description]
        categories = [dict(zip(keys, row)) for row in cursor.fetchall()]

        # Lấy tác giả
        cursor.execute("SELECT * FROM Author")
        keys = [col[0] for col in cursor.description]
        authors = [dict(zip(keys, row)) for row in cursor.fetchall()]

        # Lấy nhà xuất bản
        cursor.execute("SELECT * FROM Publisher")
        keys = [col[0] for col in cursor.description]
        publishers = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify({
            "books": books,
            "categories": categories,
            "authors": authors,
            "publishers": publishers
        })

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500