import pyodbc

def get_connection():
    connection_string = (
        "DRIVER={ODBC Driver 17 for SQL Server};"
        "SERVER=localhost;"
        "DATABASE=BTL_API;"
        "Trusted_Connection=yes;"
    )
    return pyodbc.connect(connection_string)