create view ProductsExportToCsv_V
as
select C.CategoryName as CategoryName,
       cast(1 as int) as CategoryIsActive,
       P.ProductID as ProductCode,
       P.ProductName as ProductName,
       P.UnitPrice as ProductPrice,
       P.UnitsInStock as ProductQuantity,
       case when p.Discontinued = 1 then 0 else 1 end as ProductIsActive
from Categories C
         inner join Products P on C.CategoryID = P.CategoryID