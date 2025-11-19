// Поганий приклад
let sum a b= a+b
let calc x=
    if x>0 then
      x*2 else x*3

// Гарний приклад
let sum a b = 
    a + b

let calculate x =
    if x > 0 then
        x * 2
    else
        x * 3     

// Поганий приклад
let x = 3.14
let f a b = a + b
type prs = {n:string; a:int}

// Гарний приклад
let piValue = 3.14
let addNumbers a b = a + b

type Person = {
    Name : string
    Age  : int
}

// Поганий приклад
let add a b = 
    // додаємо два числа
    a + b

// перевіряємо чи число більше нуля
let check x =
    if x > 0 then true else false

// Гарний приклад

// Використовуємо формулу для корекції показника,
// оскільки вхідні дані можуть бути нестабільними
let adjustValue x =
    let factor = 1.15
    x * factor

(* Багаторядковий коментар:
   Пояснюємо винятковий випадок,
   який не є очевидним з коду *)
let safeDivide x y =
    if y = 0 then None
    else Some (x / y)

// Поганий приклад
/// Функція
let calc x =
    x * 2

type User = {
    Name: string
    Age: int
}

// Гарний приклад
/// <summary>
/// Обчислює подвоєне значення числа.
/// </summary>
/// <param name="x">Вхідне число.</param>
/// <returns>Подвоєне значення параметра.</returns>
let doubleValue x =
    x * 2

/// <summary>
/// Модель користувача системи.
/// </summary>
type User = {
    /// <summary>Ім’я користувача.</summary>
    Name: string
    /// <summary>Вік користувача.</summary>
    Age: int
}

// Поганий приклад
let CalculateValue(X)=
    let mutable result=0
    result <- (X*2)
    result

type user_profile = {
    user_name: string;
    user_age: int;
}

let ProcessData data =
    (List.filter (fun x -> x>0) data)

// Гарний приклад
let calculateValue x =
    x * 2

type UserProfile = {
    UserName: string
    UserAge: int
}

let processData data =
    data
    |> List.filter (fun x -> x > 0)

// Поганий приклад
let isEven n =
    if n % 2 = 0 then true else false

// Гарний приклад
// Tests.fs
open NUnit.Framework

[<Test>]
let ``isEven returns true for even numbers`` () =
    Assert.IsTrue(isEven 4)

[<Test>]
let ``isEven returns false for odd numbers`` () =
    Assert.IsFalse(isEven 5)

// Мінімальна імплементація для проходження тестів
let isEven n = n % 2 = 0

// Поганий приклад
let discount price age =
    if age < 18 then price * 0.9
    else if age > 60 then price * 0.85
    else price

// Гарний приклад
let juvenileDiscount price = price * 0.9
let seniorDiscount price = price * 0.85

let applyDiscount price age =
    match age with
    | a when a < 18 -> juvenileDiscount price
    | a when a > 60 -> seniorDiscount price
    | _ -> price
