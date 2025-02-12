from datetime import datetime


def calculate_age(birth_date: str) -> int:
    
    try:
        year = int(birth_date[:4])
        month = int(birth_date[4:6]) if birth_date[4:6] != "00" else 1
        day = int(birth_date[6:]) if birth_date[6:] != "00" else 1
    except ValueError:
        print(f"error in birth_date : {birth_date}")
    try:
        birth_date = datetime(year, month, day)
    except ValueError:
        birth_date = datetime(year, 1, 1)

    today = datetime.today()

    age = (
        today.year
        - birth_date.year
        - ((today.month, today.day) < (birth_date.month, birth_date.day))
    )

    return age
