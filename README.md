# CRUDUsersApi
Web Api, реализующее CRUD методы для модели User. В качестве базы данных используется in-memory-database, в качестве провайдера используется EFcore InMemory.

# User
Модель User включает в себя следущие поля:

    Guid - Guid - Уникальный идентификатор пользователя
    Login - string - Уникальный Логин (запрещены все символы кроме латинских букв и цифр), Password - string - Пароль(запрещены все символы кроме латинских букв и цифр),
    Name - string - Имя (запрещены все символы кроме латинских и русских букв)
    Gender - int - Пол 0 - женщина, 1 - мужчина, 2 - неизвестно
    Birthday - DateTime? - поле даты рождения может быть Null
    Admin - bool - Указание - является ли пользователь админом
    CreatedOn - DateTime - Дата создания пользователя
    CreatedBy - string - Логин Пользователя, от имени которого этот пользователь создан ModifiedOn - DateTime - Дата изменения пользователя
    ModifiedBy - string - Логин Пользователя, от имени которого этот пользователь изменён RevokedOn - DateTime- Дата удаления пользователя
    RevokedBy - string - Логин Пользователя, от имени которого этот пользователь удалён
    
# Методы
## Post
    /api/Users/CreateUser
    Успешный запрос: статус Ok + сообщение о добавлении пользователя
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `UserLogin` - string 
* `UserPassword` - string	
* `UserName` - string	
* `UserGender` - integer($int32)	
* `UserBirthday` - string($date-time)
* `isAdmin` - boolean

## Put
    /api/Users/UpdateUser
    Успешный запрос: статус Ok + сообщение об изменении пользователя
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `UserLogin` - string

Необязательные параметры:
* `NewUserName` - string	
* `NewUserGender` - integer($int32)	
* `NewUserBirthday` - string($date-time)

## Put
    /api/Users/UpdatePassword
    Успешный запрос: статус Ok + сообщение об изменении пароля пользователя
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `UserLogin` - string
* `NewPassword` - string	

## Put
    /api/Users/UpdateLogin
    Успешный запрос: статус Ok + сообщение об изменении логина пользователя
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `UserLogin` - string
* `NewLogin` - string

## Put
    /api/Users/UserRecovery
    Успешный запрос: статус Ok + сообщение о востановлении пользователя
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `UserLogin` - string

## Get
    /api/Users/GetAllActiveUsers
    Успешный запрос: статус Ok + список всех активных пользователей
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 

## Get
    /api/Users/GetUserByLogin
    Успешный запрос: статус Ok + доступная информация о пользователе (имя, дата рождения, пол)
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `UserLogin` - string

## Get
    /api/Users/GetUser
    Успешный запрос: статус Ok + полная информация о пользователе
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 

## Get
    /api/Users/GetOlderUsers
    Успешный запрос: статус Ok + список всех пользователей, старших заданного возраста
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `age` - int


## Delete
    /api/Users/DeleteUser
    Успешный запрос: статус Ok + сообщение об удалении пользователя
    Неуспешный запрос: статус BadRequest + сообщение о возникшей ошибке
Обязательные параметры:
* `login` - string,
* `password` - string 
* `UserLogin` - string
* `IsHard` - bool
