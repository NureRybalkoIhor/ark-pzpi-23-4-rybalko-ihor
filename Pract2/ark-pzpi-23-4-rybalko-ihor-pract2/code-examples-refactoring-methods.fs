//Поганий код
type Student = { Name : string; Scores : int list; Attendance : int; Homework : int list }

let generateCourseReport (students: Student list) =
    let mutable report = []
    for s in students do
        let scoreSum = List.sum s.Scores
        let hwSum = List.sum s.Homework
        let avgScore = (scoreSum + hwSum) / (List.length s.Scores + List.length s.Homework)

        let mutable status = ""
        if avgScore > 90 && s.Attendance > 95 then
            status <- "Excellent"
        elif avgScore > 75 && s.Attendance > 80 then
            status <- "Good"
        elif avgScore > 60 && s.Attendance > 70 then
            status <- "Satisfactory"
        else
            status <- "Poor"

        let mutable bonus = 0
        if s.Attendance > 98 then bonus <- bonus + 5
        if avgScore > 95 then bonus <- bonus + 10

        let finalScore = avgScore + bonus
        report <- (s.Name, finalScore, status) :: report

    report

//Гарний код
type Student = { Name : string; Scores : int list; Attendance : int; Homework : int list }

let calculateAverage s =
    let scoreSum = List.sum s.Scores
    let hwSum = List.sum s.Homework
    (scoreSum + hwSum) / (s.Scores.Length + s.Homework.Length)

let determineStatus avg attendance =
    match avg, attendance with
    | a, att when a > 90 && att > 95 -> "Excellent"
    | a, att when a > 75 && att > 80 -> "Good"
    | a, att when a > 60 && att > 70 -> "Satisfactory"
    | _ -> "Poor"

let calculateBonus avg attendance =
    let bonusAttendance = if attendance > 98 then 5 else 0
    let bonusScore = if avg > 95 then 10 else 0
    bonusAttendance + bonusScore

let generateCourseReport students =
    students
    |> List.map (fun s ->
        let avg = calculateAverage s
        let status = determineStatus avg s.Attendance
        let bonus = calculateBonus avg s.Attendance
        let finalScore = avg + bonus
        (s.Name, finalScore, status))

//Поганий код
type CourseInfo = { G : int; L : int; S : int; C : int }

let f (d: CourseInfo list) =
    let mutable t = 0
    for x in d do
        let h1 = x.G * 2
        let h2 = x.L * 3
        let h3 = x.S * 1
        let h4 = x.C * 4
        let res = h1 + h2 + h3 + h4
        t <- t + res
    t

//Гарний код
type CourseInfo = { Grade : int; Labs : int; Seminars : int; Credits : int }

let calculateCourseScore (c: CourseInfo) =
    c.Grade * 2 +
    c.Labs * 3 +
    c.Seminars * 1 +
    c.Credits * 4

let calculateTotalScore (courses: CourseInfo list) =
    courses
    |> List.sumBy calculateCourseScore

// Поганий код
type Student = { Name : string; Score : int; Attendance : int }

let getPerformanceStatus student =
    if student.Score > 90 && student.Attendance > 95 then
        "Excellent"
    else if (student.Score > 75 && student.Attendance > 80) then
        "Good"
    else if (student.Score > 60 && student.Attendance > 70) then
        "Satisfactory"
    else if student.Score <= 60 && student.Attendance <= 70 then
        "Poor"
    else 
        "Undefined"

// Гарний код
type Student = { Name : string; Score : int; Attendance : int }

let getPerformanceStatus student =
    match student.Score, student.Attendance with
    | score, attendance when score > 90 && attendance > 95 -> "Excellent"
    | score, attendance when score > 75 && attendance > 80 -> "Good"
    | score, attendance when score > 60 && attendance > 70 -> "Satisfactory"
    | _ -> "Poor"
