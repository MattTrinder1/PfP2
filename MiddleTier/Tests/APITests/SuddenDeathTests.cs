using Common.Models.Business;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using MoD.CAMS.Plugins.Common;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace APITests
{
    [TestClass]
    public class SuddenDeathTests : TestBase
    {

        private string imageString = "iVBORw0KGgoAAAANSUhEUgAAAiEAAAIhCAYAAACYF2qHAAAgAElEQVR4Xu3dC9BF6zkf8L9zTk4i99txIhJJJEIal2iCoS2aSGjcRus2qFYVE0aUXhLXIOrSoUgVIxTtCEZrGGUiJEpRxKBCGkJCEOKc3K9yofNO1k7W2Wd/37f3Xrf3Xeu3Z745yTl7v+/z/p619/f/1m2/QzwIECBAgAABAgsIvMMCc5qSAAECBAgQIBAhxEZAgAABAgQILCIghCzCblICBAgQIEBACLENECBAgAABAosICCGLsJuUAAECBAgQEEJsAwQIECBAgMAiAkLIIuwmJUCAAAECBIQQ2wABAgQIECCwiIAQsgi7SQkQIECAAAEhxDZAgAABAgQILCIghCzCblICBAgQIEBACLENECBAgAABAosICCGLsJuUAAECBAgQEEJsAwQIECBAgMAiAkLIIuwmJUCAAAECBIQQ2wABAgQIECCwiIAQsgi7SQkQIECAAAEhxDZAgAABAgQILCIghCzCblICBAgQIEBACLENECBAgAABAosICCGLsJuUAAECBAgQEEJsAwQIECBAgMAiAkLIIuwmJUCAAAECBIQQ2wABAgQIECCwiIAQsgi7SQkQIECAAAEhxDZAgAABAgQILCIghCzCblICBAgQIEBACLENECBAgAABAosICCGLsJuUAAECBAgQEEJsAwQIECBAgMAiAkLIIuwmJUCAAAECBIQQ2wABAgQIECCwiIAQsgi7SQkQIECAAAEhxDZAgAABAgQILCIghCzCblICBAgQIEBACLENECBAgAABAosICCGLsJuUAAECBAgQEEJsAwQIECBAgMAiAkLIIuwmJUCAAAECBIQQ2wABAgQIECCwiIAQsgi7SQkQIDCpwJuTXHvCDG9J8sIkz0ryuSe8zlMJDBIQQgbxeTEBAgSqEviyJF87UkUvTvIuI41lGAIHBYQQGwYBAgTWIfC8JO/RW8oPJvn0K5b2R0neOcntklz0+0AYWcf2UeUqhJAq26IoAgQInCTwpiTX9V7xEUmecdIIt3zyXye5fu/1wsgAUC89LCCE2DIIECDQrsDjknxHr/xXJbnLiMs5FEZuSvJOI85hqA0LCCEbbr6lEyDQtMCvJPmgEw+/nLvgQ2HkC5M85dwBvY5AERBCbAcECBBoT+C1SW7flf03Sf5BkhJKpn68McltepOU81AeMvWkxl+vgBCy3t5aGQEC6xP47iSf3VvWXyS598zL/PEkH9ebs4SgUy4Hnrlc09UsIITU3J1partrkpcmuWaa4ZsY9W97f0EWixubqFqRWxb49u7+Hf2TT781yRctiLJ7H+1KcHhmwWa0OrUQ0mrnzq/7q5I86fyXb+aV5QO2/JS/8vq7nzcDYKFVCBwKH2XvxyfMdPjlKoTXJXnH3pMcnrlKzH+/hYAQsr0Nwp6QcXq+Cynln++T5LnjDGsUAm8T+LEkH9/z+JMkH5PkOZUZOTxTWUNaKkcIaalbddRaToD7e0mefOCQTmuHePq7k8d+L/RDykvcebKOjbehKr4nyWd19dYaPvY5HZ5paAOrpdSxP3hrWZc62hB4VJL3T/LVezdaKtWXbXOt2+f+h3W/W+Xwz1WP8nqHiK5Save//1CST+nK/94k/7KhpewfnvnZJI9pqH6lziyw1g/5mRlNV4lAuUvkuyd5tySfmuSGC+pac8A5pxWXhaJzxtvia8ols3cauPCHJfm+JOWf5dFaANktf//wjN8zAzeMNb/cxrHm7lrbVALv250Hcv8k5accp7/HEZO1drjqiCV5SpISQO44UKKcLF5OGi+PcvilfAldOSTT6uMFSR7QFf/EJN/Y6kLUPa2AEDKt7270n0zy2N5U5Wu2bzvP1GapUKB8bfp+IDn3XguPSHLf7pyT8kVkZe/P3btbd3/YkZdiC0fnbSTn9mx/ttKnn+/+5bd1YeQV55VU1at2e9jK9t6/tLiqIhWzrIAQMo///u7usb/fYZ5VmGVsgfJLbP89WO5IKaCOLX3+eL/UnYjdH6G8n8t3p9x8/rC3eGW5Yq3c8+P7k/yvkcasYZjyx9buJmZ+19TQkQprsGFM35RyM6zyl+nuIYBMb97SDM9P8qADBf9Zt4ejpbWsrdZDIfHZST5gbQudcD27P8BemaSELQ8CtxAQQqbdIF6W5G4CyLTIKxn90BeElQ9wh0rmb/Ch8DHGeR/zr2T5Gae8DH751algsIAQMpjwwgFevpf87QGZznpNI7/pwPHzsc49WJPT2Gt5VpJybsb+ZyL7YdLPSPLobggnqA6zXOWrhZDp2tr/C0AAmc55rSMfOnnV+SLjd7scJrjzgWHL+7dc8VT+mPAYJmBvyDC/Vb9aCJmmvf1d6+Uv2+unmcaoGxA4dGig3IH1XhtY+5RLPBTyynz2fIyvXq70uUs3rN854/s2PaINYpr2Sf7TuG511BcdOEm1bGPl8tzf3CrKGesuVx29/oI78b76gj0iZ0zjJRfsWSr/2t48m8ctBISQ8TeI/l6Qcoma22uPb7zVEQ+dvGobO25ruGjPx68cuAT3uBE96xSB/h49v3dOkVv5c20M4zfYXpDxTY14S4H9X6jex5dvIfuHtMp79MYkN9mwZhN4QpJv6GbzfTKzsdc/kQ+vcXvUvyTXX6jj2hrtlgL9vSJ/muRdAd1KoNyXon9iqUuel91I/IG2rH+Vswsh47al/xcq23FtjXZrgd2Hul+ut7Z5ZpJH9v61E06Xfwf5Ppnle1BdBX5RjtsSvxTG9TTa8YcZvJffbrV/rxV7Jet5J+0+I4XCenqyaCU+uMbjL5bljVUePvTGczXSxQLl1u7v0v1nVx28FWL/fJlyE7JH2YiqEXBIpppW1FGIEDJeH3xZ03iWRjpewN63t1vtn4Dq8+347WiuZ+62V7fBn0u88nm8ScdrkIQ/nqWRjhdwHtJbvxjNCajHbzNLPlMIWVK/wrmFkPGa4ljneJZGOl7gAUnKCX/lscXDgE5APX5bqeGZQkgNXaioBiFknGb0L5f8wCS/Ps6wRiFwlMBW98I5AfWozaOqJ+22Vb97qmrLcsXYEMaxdzfAcRyNcp5A/5fxNUn6oeS8Eet/lRNQ6+/RoQqFkDb7NlnVQsg4tE4OHMfRKOcLbOlwoBNQz99Oln6lELJ0ByqbXwgZ3pBy6+d7dsO8qvdtkcNHNgKB4wW2cEjGCajHbw81PvM1Se7QFeZ3T40dWqAmG8JwdFcnDDc0wnCB/nlJ75bkhcOHrGoEJ6BW1Y6zihFCzmJb94uEkOH93cJfoMOVjDCHwFoPyTgBdY6tZ/o5hJDpjZubQQgZ3jIhZLihEYYL/EWSe3XDrOnuqU5AHb5t1DKCz8paOlFRHULI8GZ4Yw03NMJwgTVeoeUE1OHbRU0j+KysqRuV1CKEDG+EN9ZwQyMMF1jTFVpOQB2+PdQ4gs/KGruycE1CyPAGeGMNNzTCMIH+Sal/nuQ+w4Zb9NVOQF2Uf9LJ3S11Ut42BxdChvdNCBluaIRhAmvaBvtr2eJt6IdtCfW++uuTPLEr7/VJbl9vqSqbU0AIGa69pl8AwzWMMLdAeQ+XcyfKo/zz2rkLGHG+/kmoz0ryqBHHNtSyAj4nl/WvdnYhZHhrvLmGGxrhfIH+L+7Wb9nuvXT+dlDzK8serV04/qskN9ZcrNrmFRBChnv74BxuaITzBday/fWvhHlGko84n8QrKxLoH4Yp22oJyh4E3iYghAzfGNbyS2C4hBHmFnhlkjt3k7Z+b5A1Xd0z93ZQ83w+H2vuTgW1CSHDm+BNNtzQCKcJPCHJN+y9pOX3svfQaf1v5dn9wzC/nuQDWylcnfMJtPzBNZ/S5TOt8SZRtdiq49YC+zfwKs9o+SqSn0nymG6ZrZ9Ya3t9u8CvJfmA7v86DGPLuFBACBm+cZRj10/vhmn5l8FwCSNMKbB/+/Iy1xp+adsLMuVWs9zY+rqcfVMzCyHjtMsbbhxHo9xaYP/L28ozyvZ2Q5KXNg5W6r+7AN94Fy/fW1cOG37J6lZoQaMJCCHjUPaPfd4pSfm2SA8CQwT6d0Htj/PhScpdRdfwEN7X0MVbruElSd6p+1dl791161uiFY0pIISMp7nWr1EfT8hIxwi8OMk7H3ji2u6v0D+89LIk9zgGx3OqFxAsq29RXQUKIeP1wwmq41lucaT7JXlhkv33ZOuX3l7US7+s1reV93vqMMz6+jvJioSQ8Vj792x4ee9Y93gzGGltAuUwXrl506H34ZpPcu7vBXFjsnVs1f3DMG9I8o7rWJZVTC0ghIwr7IZL43qubbTLQsdurWu44uWqvnmfXCXU1n/v3xW1VO73Slv9W7RaG8u4/A7JjOvZ+mjHhI6yxrLd/EWS+7S+4CPq9x45Aqmxpzi01ljDaipXCBm3G/17hpSRX5XkLuNOYbSKBU4JHWX39b0rXssUpbkx2RSqy45ZDr3ctithbSdPLyu7kdmFkPEb3b9ct4zuMrXxjWsYsbx3yj08Ljqno19j+ev/piT3qqHwBWvwF/OC+BNM3d+r5a6oEwBvYUghZJou909SLTN4g07jPOeop4aOciOu3f0S5qyz1rn6NybbwnkvtfZhrLr2vz7A75KxZDc2jg1nuoaX700o35/Qf3xokl+cbkojjyhwxySvOHJPRwmZ5Yoo97q4uAH2goy4cS48VL+X/sBauBmtTy+ETN/B/b8YnCcyvfk5M5waOsrerrudM9EGX9PfC+LGZO1uAP8xyRf1yhdA2u1lNZULIfO0wnki8zifMkv/fI6r3gflw/bVTjI+hfcWz7UX5Gy6al64H0CEyWpa03YhV334tr26uqp3nsgy/SjfwXLtkYdVdhUKHeP1yo3JxrNcaqT+nqxSgwCyVCdWOK8QMn9T+38Vltn1YLweHHuJ7P6MQsd4PThku/t3tvXpnKcaeT+AfEuSL55qMuNuT8CHwjI9d57I+e7l3I1yEmi5NLb8nPIoYaP8/HKSDznlhZ57lkB/O3d79rMIF33R/ueUALJoO9Y5uRCyXF/LOQn9r7l2P5Fb96IcSilGZTs9dVvd7d14cJJyYzCP+QXcnn1+87FmdAnuWJLGuVTg1A92nOMKOE/krZ5PTPK1J563setE+UVXPjD/SZKfGLc9Rhsg0D8XxOfMAMgFXuoS3AXQtzqlD4flO39jkr/cK+OBSV6wfGmjV1AOgTyzO1H01G1vdyilfFnWl49emQHHFrAXZGzR6cdzCe70xmbYEzj1FwHA6QTWdJ5IOUG0PMr2dep5G+V1xeI1LomdbmObeOSfSvLYbo6bk9ww8XyGHy7gEtzhhkY4Q0AIOQNtwpfMdZ7Ik5KUO7qWL9zbPQ6ddzH19lHCRvm5zYSmhp5fwDflzm8+ZMb9K2B+O8n7DRnQawkcKzD1L5lj6/C8twuUKz/uujKQ3aGU8tdx+SZVj3ULOBTTTn/Lyd/X98p1BUw7vVtFpUJInW0sl6GWO3TW9NgFiX5N5bDL1yX5Lleg1NSqRWvpH4r56SQftWg1Jr9MwBUwto/FBYSQxVugAAKrEnAopo12umliG31afZVCyOpbbIEEZhVwKGZW7rMmcwnuWWxeNIWAEDKFqjEJbFPgpiT37JbuUEx928D+FTBvTHLb+spU0ZYEhJAtddtaCUwr4FDMtL5DRv+tJA/rDeBL6IZoeu1oAkLIaJQGIrB5gd1u/hJGyjcXe9Qh4Evo6uiDKg4ICCE2CwIExhAou/Z393vxuTKG6Dhj+BK6cRyNMpGAD4uJYA1LYGMC/ZMdfa7U0XyX4NbRB1VcIuDDwuZBgMAYArsQUu7627/51RhjG+N0AVfAnG7mFQsICCELoJuSwMoEfGNuPQ31JXT19EIlRwgIIUcgeQoBApcKOBRTxwayf/jFFTB19EUVDsfYBggQmFDAoZgJca8Y+j8neVz3jdX9pwogy/XEzCcI2BNyApanEiBwKwGHYpbZKC4KHyUQXrNMSWYlcLqAEHK6mVcQIPB2Abdpn3dr+L0kf+fAlO7NMm8fzDaSgBAyEqRhCGxQoP+NuTcnuWGDBnMtuX8flv6crkaaqwPmmURACJmE1aAENiHgNu3Tt/mi8PHcJA+dfnozEJhWQAiZ1tfoBNYs4FDMdN3dv9JlN5PwMZ25kRcQEEIWQDclgRUI9A/F+MbccRp60cmmZfTvSPL540xjFAL1CAgh9fRCJQRaEnAoZrxuudJlPEsjNSYghDTWMOUSqETAoZjhjXCly3BDIzQuIIQ03kDlE1hA4KYk9+zmdSjm9Aa40uV0M69YqYAQstLGWhaBCQUcijkP15Uu57l51YoFhJAVN9fSCEwksDsU4wZZxwG70uU4J8/aoIAQssGmWzKBAQL9v+Z9flwM6UqXARuZl25HwIfIdnptpQTGEPCNuZcrutJljK3MGJsREEI202oLJTCKgG/MPczoSpdRNi+DbE1ACNlax62XwPkCDsXc2s6VLudvT15JIEKIjYAAgWMFHIp5u5QrXY7dajyPwCUCQojNgwCBYwUciklc6XLs1uJ5BI4QEEKOQPIUAgTyliTXdA5b+9xwpYs3AIGJBLb2YTIRo2EJrF5gi7dpd6XL6jdrC1xaQAhZugPmJ1C/QP8bc29OckP9JQ+q0JUug/i8mMDxAkLI8VaeSWCrAlu5TXv/kFO/129Kcv1Wm2/dBKYUEEKm1DU2gXUIrPlQzB8keVBy8ErB5yZ56DpaaBUE6hQQQursi6oI1CLQPxSzpm/MvegqlxK4vjfJZ9fSAHUQWLOAELLm7lobgeECazoUU/Z6vPsFJG9OcpvhXEYgQOAUASHkFC3PJbA9gdYPxTw1yWddcLjFXo/tbc9WXJmAEFJZQ5RDoDKBXQgpe0Suray2y8p5dZI7XvCE1yS5U0NrUSqB1QoIIattrYURGEWgpRBir8coLTcIgfkEhJD5rM1EoEWBFkJIuYT2ugtwn5/kwS3Cq5nAFgSEkC102RoJnC9Qawi5aq/H7hbz56/cKwkQmFxACJmc2AQEmhaoLYTY69H05qR4ArcUEEJsEQQIXCZQQwi57IZirZ0wa2sjQKAnIITYHAgQqDWEXHQb9VKvcz1stwRWICCErKCJlkBgQoHdnpAyxX9P8okTzlWGttdjYmDDE6hJQAipqRtqIVCfwI8m+YReWWXvxEVXopxS/Wv3vhSu3IPk0OeRG4qdouq5BBoTEEIaa5hyCSwgsB9EdiWUgFBCye7Rv5nZ0M8Wt1FfoNGmJDC3wNAPirnrNR8BAssIXBRExqzGXo8xNY1FoAEBIaSBJimRQGUCT0vyyUmuuhdHfy/JXye5Q2XrUA4BAgsLCCELN8D0BAgQIEBgqwJCyFY7b90ECBAgQGBhASFk4QaYngABAgQIbFVACNlq562bAAECBAgsLCCELNwA0xMgQIAAga0KCCFb7bx1EyBAgACBhQWEkIUbYHoCBAgQILBVASFkq523bgIECBAgsLCAELJwA0xPgAABAgS2KiCEbLXz1k2AAAECBBYWEEIWboDpCRAgQIDAVgWEkK123roJECBAgMDCAkLIwg0wPQECBAgQ2KqAELLVzls3AQIECBBYWEAIWbgBpidAgAABAlsVEEK22nnrJkCAAAECCwsIIQs3wPQECBAgQGCrAkLIVjtv3QQIECBAYGEBIWThBpieAAECBAhsVUAI2WrnrZsAAQIECCwsIIQs3ADTEyBAgACBrQoIIVvtvHUTIECAAIGFBYSQhRtgegIECBAgsFUBIWSrnbduAgQIECCwsIAQsnADTE+AAAECBLYqIIRstfPWTYAAAQIEFhYQQhZugOkJECBAgMBWBYSQrXbeugkQIECAwMICQsjCDTA9AQIECBDYqoAQstXOWzcBAgQIEFhYQAhZuAGmJ0CAAAECWxUQQrbaeesmQIAAAQILCwghCzfA9AQIECBAYKsCQshWO2/dtQm8Jck1Sa5LUv63BwECBFYvIISsvsUW2IjA33Z1viLJ3RqpWZkECBAYJCCEDOLzYgKjCexCyKuS3GW0UQ1EgACBigWEkIqbo7RNCQghm2q3xRIgUASEENsBgToEdiHkZUnuUUdJqiBAgMC0AkLItL5GJ3CsgBByrJTnESCwGgEhZDWttJDGBYSQxhuofAIEThcQQk438woCUwjsQshLk9xzigmMSYAAgdoEhJDaOqKerQrsQshLktxrqwjWTYDAtgSEkG3122rrFRBC6u2NyggQmEhACJkI1rAEThQQQk4E83QCBNoXEELa76EVrENACFlHH62CAIETBISQE7A8lcCEAkLIhLiGJkCgTgEhpM6+qGp7AkLI9npuxQQ2LyCEbH4TAFCJgBBSSSOUQYDAfAJCyHzWZiJwmYAQYvsgQGBzAkLI5lpuwZUK7ELInyZ510prVBYBAgRGFRBCRuU0GIGzBXYh5E+S3P/sUbyQAAECDQkIIQ01S6mrFhBCVt1eiyNA4JCAEGK7IFCHgBBSRx9UQYDAjAJCyIzYpiJwiYAQYvMgQGBzAkLI5lpuwZUKCCGVNkZZBAhMJyCETGdrZAKnCAghp2h5LgECqxAQQlbRRotYgYAQsoImWgIBAqcJCCGneXk2gakEhJCpZI1LgEC1AkJIta1R2MYEhJCNNdxyCRBIhBBbAYE6BISQOvqgCgIEZhQQQmbENhWBSwSEEJsHAQKbExBCNtdyC65UQAiptDHKIkBgOgEhZDpbIxM4RUAIOUXLcwkQWIWAELKKNlrECgSEkBU00RIIEDhNQAg5zcuzCUwlIIRMJWtcAgSqFRBCqm2NwjYmIIRsrOGWS4CAS3RtAwRqERBCaumEOggQmE3AnpDZqE1E4FIBIcQGQoDA5gSEkM213IIrFRBCKm2MsggQmE5ACJnO1sgEThHYhZA/TvKAU17ouQQIEGhVQAhptXPqXpvALoT8fJJHrm1x1kOAAIFDAkKI7YJAHQK7EPJ5Sb6zjpJUQYAAgWkFhJBpfY1O4BiBcvjlBd0TvSePEfMcAgRWIeADbxVttIjGBX44yScLIY13UfkECJwsIIScTOYFBEYXeH6SBwkho7sakACBygWEkMobpLxNCLwyyZ2FkE302iIJEOgJCCE2BwLLC7wpyXVCyPKNUAEBAvMKCCHzepuNwCGBtyS5RgixcRAgsDUBIWRrHbfeGgV2l+eWMLLbI1JjnWoiQIDAqAJCyKicBiNwlsAuhLwuyR3OGsGLCBAg0KCAENJg05S8OoFdCPnzJPdZ3eosiAABAhcICCE2DQLLC+xCyM8meczy5aiAAAEC8wgIIfM4m4XAZQK7EPI5SZ6KigABAlsREEK20mnrrFXALdtr7Yy6CBCYXEAImZzYBAQuFXDLdhsIAQKbFRBCNtt6C69EwC3bK2mEMggQmF9ACJnf3IwE+gJu2W57IEBgswJCyGZbb+GVCLhleyWNUAYBAvMLCCHzm5uRQF/ALdttDwQIbFZACNls6y28EgG3bK+kEcogQGB+ASFkfnMzEugL7ELIa5PcEQ0BAgS2JCCEbKnb1lqjwC6EvCjJ/WosUE0ECBCYSkAImUrWuASOE9iFkKcn+UfHvcSzCBAgsA4BIWQdfbSKdgV2IeTTkjyt3WWonAABAqcLCCGnm3kFgbEE3LJ9LEnjECDQpIAQ0mTbFL0SgR9M8qndWrwXV9JUyyBA4HgBH3zHW3kmgbEFnpfkPYSQsVmNR4BAKwJCSCudUucaBdyyfY1dtSYCBI4WEEKOpvJEAqMLvDnJtUnKyanXjD66AQkQIFC5gBBSeYOUt2qB3ZUxr09y+1Wv1OIIECBwQEAIsVkQWE5gF0L+oHduyHLVmJkAAQIzCwghM4ObjkBPYBdCvibJk8gQIEBgawJCyNY6br21CHx0kp/sivE+rKUr6iBAYFYBH36zcpuMwNsEfjXJBwohtggCBLYsIIRsufvWvqTAq5LcSQhZsgXmJkBgaQEhZOkOmH+rAi7P3WrnrZsAgbcJCCE2BgLLCLg8dxl3sxIgUJGAEFJRM5SyKYFdCPn9JO+5qZVbLAECBByLtg0QWFTA5bmL8pucAIEaBOwJqaELatiagMtzt9Zx6yVA4KCAEGLDIDC/wK8neX97I+eHNyMBAnUJCCF19UM12xBwee42+myVBAhcISCE2EQIzC/g8tz5zc1IgECFAkJIhU1R0uoFXJ67+hZbIAECxwgIIccoeQ6BcQVcnjuup9EIEGhUQAhptHHKblpgF0K+MsmTm16J4gkQIDBAQAgZgOelBM4QcHnuGWheQoDAOgWEkHX21arqFXB5br29URkBAjMLCCEzg5tu8wIuz938JgCAAIGdgBBiWyAwr4DLc+f1NhsBAhULCCEVN0dpqxRwee4q22pRBAicIyCEnKPmNQTOF9iFkOclecj5w3glAQIE2hcQQtrvoRW0JbALIV+a5OvbKl21BAgQGFdACBnX02gELhNwea7tgwABAj0BIcTmQGA+gd9I8vBuOu+9+dzNRIBApQI+CCttjLJWKeDy3FW21aIIEDhXQAg5V87rCJwu4PLc0828ggCBFQsIISturqVVJ+Dy3OpaoiACBJYUEEKW1Df31gR2IeS5SR66tcVbLwECBPYFhBDbBIH5BFyeO5+1mQgQaEBACGmgSUpchYDLc1fRRosgQGBMASFkTE1jEbhY4BeSfEj3n73vbCkECBBI4sPQZkBgHoGXJbmbEDIPtlkIEGhDQAhpo0+qbF9gd3num5Jc3/5yrIAAAQLDBYSQ4YZGIHCMwO6k1JuT3HDMCzyHAAECaxcQQtbeYeurRWAXQn4myUfWUpQ6CBAgsKSAELKkvrm3JLALIQ9O8vwtLdxaCRAgcJGAEGLbIDC9wLcleXw3jffc9N5mIECgEQEfiI00SplNC/xhkgcKIU33UPEECEwgIIRMgGpIAnsCr09yuyTlkMw1dAgQIEDgrQJCiC2BwPQCf9O910oYuf3005mBAAECbQgIIW30SZVtC+xOSn1ekhS50zcAABWGSURBVIe0vRTVEyBAYDwBIWQ8SyMRuEhgF0K+LsmXYSJAgAABh2NsAwTmEHhUkp/rJhL65xA3BwECzQj4UGymVQptVOCZSR4phDTaPWUTIDCpgBAyKa/BCeSlSe4uhNgSCBAgcGsBIcRWQWBagd0X170lyXXTTmV0AgQItCUghLTVL9W2J7C7PNcX17XXOxUTIDCxgBAyMbDhNy+wuzKmnJz66M1rACBAgEBPQAixORCYVmAXQr4wyVOmncroBAgQaEtACGmrX6ptS+BBvW/M9V5rq3eqJUBgBgEfjDMgm2KzAr49d7Ott3ACBI4REEKOUfIcAucJ/FaSh3Uv9V47z9CrCBBYsYAPxhU319IWF9jdI6RcIXPt4tUogAABApUJCCGVNUQ5qxJ4Y5LbJHldkjusamUWQ4AAgREEhJAREA1B4AKB3T1CXpTkfpQIECBA4JYCQogtgsB0ArvLc38qyUdPN42RCRAg0KaAENJm31TdhsAuhHxzkn/TRsmqJECAwHwCQsh81mbansAuhPyHJE/Y3vKtmAABApcLCCG2EALTCQgh09kamQCBFQgIIStooiVUKyCEVNsahREgUIOAEFJDF9SwVgEhZK2dtS4CBEYREEJGYTQIgYMCQogNgwABApcICCE2DwLTCexCyL9N8k3TTWNkAgQItCkghLTZN1XXL/CIJM/uynx0kp+rv2QVEiBAYF4BIWReb7NtR+A5Sd4ryV8luXE7y7ZSAgQIHC8ghBxv5ZkEjhX44CS/3D35k5L86LEv9DwCBAhsSUAI2VK3rXUOgbsm+Z0k903yu0nee45JzUGAAIEWBYSQFrum5poFyvfEPDbJc5M8PMkbai5WbQQIEFhSQAhZUt/caxP4giRP6YJH2QPyh2tboPUQIEBgTAEhZExNY21Z4GFJfi3J9Uk+M8n3bxnD2gkQIHCMgBByjJLnELha4IeTfHKS/5bkM65+umcQIECAgBBiGyAwjkC5F0gJIY9P8rpxhjQKAQIE1i0ghKy7v1ZHgAABAgSqFRBCqm2NwggQIECAwLoFhJB199fqCBAgQIBAtQJCSLWtURgBAgQIEFi3gBCy7v5aHQECBAgQqFZACKm2NQojQIAAAQLrFhBC1t1fqyNAgAABAtUKCCHVtkZhBAgQIEBg3QJCyLr7a3UECBAgQKBaASGk2tYojAABAgQIrFtACFl3f62OAAECBAhUKyCEVNsahREgQIAAgXULCCHr7q/VESBAgACBagWEkGpbozACBAgQILBuASFk3f21OgIECBAgUK2AEFJtaxRGgAABAgTWLSCErLu/VkeAAAECBKoVEEKqbY3CCBAgQIDAugWEkHX31+oIECBAgEC1AkJIta1RGAECBAgQWLeAELLu/lodAQIECBCoVkAIqbY1CiNAgAABAusWEELW3V+rI0CAAAEC1QoIIdW2RmEECBAgQGDdAkLIuvtrdQQIECBAoFoBIaTa1iiMAAECBAisW0AIWXd/rY4AAQIECFQrIIRU2xqFESBAgACBdQsIIevur9URIECAAIFqBYSQalujMAIECBAgsG4BIWTd/bU6AgQIECBQrYAQUm1rFEaAAAECBNYtIISsu79WR4AAAQIEqhUQQqptjcIIECBAgMC6BYSQdffX6ggQIECAQLUCQki1rVEYAQIECBBYt4AQsu7+Wh0BAgQIEKhWQAiptjUKI0CAAAEC6xYQQtbdX6sjQIAAAQLVCggh57fmb5OUn2vOH8IrCRAgQIDAdgWEkPN6/7Akv9V76ecm+e7zhvIqAgQIECCwTQEh5Py+/3mSe/de/rQkn3b+cF5JgAABAgS2JSCEDOv3s5M8ojfEi5Lcb9iQXk2AAAECBLYhIIQM7/OXJ3lyb5g3Jrnt8GGNQIAAAQKVCtwxyRcn+aIkP5vkkyqts/qyhJBxWlQOy5TDM7uHE1bHcTUKAQIEahL4mC54fHDvj83vSvK4mopsqRYhZNxulfDRf7xLkhePO4XRCBAgQGBGgRI4npTkQ5LcrjfvryT5/CS/PWMtq5tKCBm/pW9Ocm1v2PdJ8pzxpzEiAQIECEwk8N5JvirJo5PcqTdHOdz+S10oKf/0GCgghAwEvODlr0hyl95/e1OS66eZyqgECBAgMILAfbvz+8ohl7v3xit7uMstGZ6S5AdGmMcQPQEhZLrN4Q+TPHBv+N9M8vDppjQyAQIECJwgcLcueHxCkhv3XveCJP8lyb8/YTxPPVFACDkR7MSnl70hLztwV9XPSfLUE8fydAIECBAYLvCOSb4kyWccuKXCTUl+NMlXJnnp8KmMcJWAEHKV0Dj//WuSfMXeUA7RjGNrFAIECFwlUM7T+8Ik5Q/AByfp/+57bZKnJym3W3jeVQP57+MKCCHjel412h8fSN6/m6ScBOVBgAABAuMKfFOSj+0Ojfe/56tcQPB/upNPnzXulEY7RUAIOUVrnOfeJskbHKIZB9MoBAgQ6An8s26Px0MPXAxQTjAtf/R9R5Jybw+PCgSEkOWa8KUHTngq6byEFA8CBAgQuFqgfE3GNyf5sCT3uODpr0zyX5M8/urhPGNuASFkbvFbz/cHSd5971//XpL3Wr40FRAgQKAqgfI7q/wBV/Z4vNvePZl2hZbz7crVieXk/2+pqnrF3EpACKljoyh9KDfBuW6vHFfR1NEfVRAgsJzAB3V7jT8gyR0OlFEOs9yc5KeTPDHJXy5XqplPFRBCThWb9vmfneS796Yoh2jKjc72bwk/bSVGJ0CAwDIC5Q6l35jk45K8896VLLuKXt/dQKzcTv3nlinTrGMICCFjKI4/RvkugvfdG/bnkzxy/KmMSIAAgcUFPq+7fPYhF9xd+i1J/izJjyR5wuLVKmA0ASFkNMpJBiqHaPZPVC17Rj69ezNOMqlBCRAgMLHAA5J8Q5IP37tFen/alyf5he7GYu7fMXFDlhpeCFlK/vh5/2l3ZvehV7wkyb2OH8ozCRAgsJjAVyf5tCT3v+SE0hI2vrP7WaxQE88nIITMZz10ptt2J1zd9cBA5XyRp3V7SIbO4/UECBAYKlDObSuHj8s5HeU26Yd+15TPrXIS6f/sDsUMndPrGxQQQhpsWvc10uW7Dfp3ANyt5K+TPDaJuwC22VtVE2hJoBwa/ufdOWzly+DK7dEve5RbpP9Gd4v0X2ppoWqdRkAImcZ1zlGfn+RBF0z4wu5a+jnrMRcBAusTKAGj/OHzkUne9ZK9G/2V/02SV3Xfx/KDSb59fSxWNFRACBkqWM/ry4fDT1xwZnn5MPjWJP+6nnJVQoBApQIf2p0M+n7dSaP79y86VHbZA1sOrZS9G09O8vuVrk1ZlQkIIZU1ZKRyfry7xv7QcK/rdp2WOwp6ECCwXYFyrsZXdYdvy9Uqt7/g3I39vRuvTlLu9Fy+8r58QZx7GG13Gxq8ciFkMGHVA9wlyYuS3PmCKn/nwP1Iql6Q4ggQOEvgg7v7a7x/knse+R1V5RYBf9V92+zXJimfFx4ERhUQQkblrHqwr+j+6jl0Mmu5EdC/csy26v4pjsAxAuW+QuW7VT6+Ox+s3Ob80Ht+f+9GOWG07B39se7+HeV+RB4EJhcQQiYnrnKCP05Svn3y0KPsOXlZkhJayqVzHgQI1Cnwd7vAUfZylG+QLV/vcNWjfLnbTd0VKmXvxrOveoH/TmBKASFkSt36x/7oJP/jiA+vcsy37Jr9oyRfl6Sc6e5BgMA8AmVvxgOTfE+SBycp361y1d6N8p59TZJyhVz5bhUnpc/TK7OcKCCEnAi24qc/M8k/POLEtD7BLpz8abcL93tX7GNpBKYSeFyS8gfBQ7s9GuWE0avut9GvpRw6eWm3V6PcCv2XpyrUuATGFhBCxhZdz3h37A7HlF2+5X+fuq2U3b7lC6d+IEm5XbMHga0KlG+E/fvd/XzKyeLlvI2r9mRcZFXO3fiTJOWPhsdvFdS61yNw6i+W9azcSs4VeKdut3D5UC1X3ZzyF1uZs4STcj+BH/JtmOe2wOsqEij35/nUJA9Pcu8k5dBJua/GuZ+tZa9G+Zr6ct7Gc7p7/3xfRetVCoFRBc59o4xahMFWIXC7JOVwzGOSHHP75v1Flw/f8sFb7nFSvtbbg8DSAuXz8QuSfFSS9+wOlZTt/NTgvVtHOXxZQni5z0Y5hPlr3U0EfUPs0p02/2ICQshi9JuZuGxj35XkY7v7Exxz98U+Trl8uBzv/ukk/8KNkTaz3cyx0HKFWLk0vezVK/+77NkrV5ic+7lY7kz8hu7qsvJ1Cs9I8p+SlEMoHgQIHBA4980Gk8AYAt+c5BOT3OvImyfth5OXdH+hlr8sPQjsC3xKkn/c3ZCvbGPljqCnhuD+mGVvXbnjcDmc+H+7K8t+BDsBAucLCCHn23nldALlHiWfmeQ+Z4ST6ap6+8i721Tv/7P8JVx+yr8vP2UvTvkpv7zKP8uu+PJT/louP+X7Nsrx/3IpZfl5RZJXJrm52/vz4t4vvDnW1docX5bkw5I8pDsEWA6VnHvC5+5Kr/KFay9I8qvdXpLWTNRLoCkBIaSpdm2+2HI45t8luf/A3eZbhex/x0c/QJXgVB67ELX7Zz88lf9d7hVT9gSUf5YQVQ4zlJ/yi3sXnl7eBaeyl+p/D4Au52CUQyWP6PpdrtAacqikhMBSczm09/+6w3tPGVCflxIgMIKAEDICoiGaE3hUkhu6w0B3774ptJwPUC6fLLvsyxUOt+2+rrz84is/5bLKsiu//JQTE8tf3P2f8l7a/RSQ/nvL+2zaTWR3wmcJSOXy1fIdJ09L8vRppzU6AQJDBXw4DhX0egLzCbxvkvt2AerGJHftAlS5g2YJUSVAlZ9ys6sSonYBqh+eyv/ehaddaNodwth9Huz/c74VHp6p7Jkph67K3pZy195f7G5XvnRd5idAYKCAEDIQ0MsJEDhKoHy/SbmPRrnPTPmek7IHqlzKXQ6zlBBVglMJUOW8mHKy51OPGtWTCBBoWkAIabp9iidAgAABAu0KCCHt9k7lBAgQIECgaQEhpOn2KZ4AAQIECLQrIIS02zuVEyBAgACBpgWEkKbbp3gCBAgQINCugBDSbu9UToAAAQIEmhYQQppun+IJECBAgEC7AkJIu71TOQECBAgQaFpACGm6fYonQIAAAQLtCggh7fZO5QQIECBAoGkBIaTp9imeAAECBAi0KyCEtNs7lRMgQIAAgaYFhJCm26d4AgQIECDQroAQ0m7vVE6AAAECBJoWEEKabp/iCRAgQIBAuwJCSLu9UzkBAgQIEGhaQAhpun2KJ0CAAAEC7QoIIe32TuUECBAgQKBpASGk6fYpngABAgQItCsghLTbO5UTIECAAIGmBYSQptuneAIECBAg0K6AENJu71ROgAABAgSaFhBCmm6f4gkQIECAQLsCQki7vVM5AQIECBBoWkAIabp9iidAgAABAu0KCCHt9k7lBAgQIECgaQEhpOn2KZ4AAQIECLQrIIS02zuVEyBAgACBpgWEkKbbp3gCBAgQINCugBDSbu9UToAAAQIEmhYQQppun+IJECBAgEC7AkJIu71TOQECBAgQaFpACGm6fYonQIAAAQLtCggh7fZO5QQIECBAoGkBIaTp9imeAAECBAi0KyCEtNs7lRMgQIAAgaYFhJCm26d4AgQIECDQroAQ0m7vVE6AAAECBJoWEEKabp/iCRAgQIBAuwJCSLu9UzkBAgQIEGhaQAhpun2KJ0CAAAEC7QoIIe32TuUECBAgQKBpASGk6fYpngABAgQItCsghLTbO5UTIECAAIGmBYSQptuneAIECBAg0K6AENJu71ROgAABAgSaFhBCmm6f4gkQIECAQLsCQki7vVM5AQIECBBoWkAIabp9iidAgAABAu0KCCHt9k7lBAgQIECgaQEhpOn2KZ4AAQIECLQrIIS02zuVEyBAgACBpgWEkKbbp3gCBAgQINCugBDSbu9UToAAAQIEmhYQQppun+IJECBAgEC7AkJIu71TOQECBAgQaFpACGm6fYonQIAAAQLtCggh7fZO5QQIECBAoGkBIaTp9imeAAECBAi0KyCEtNs7lRMgQIAAgaYFhJCm26d4AgQIECDQroAQ0m7vVE6AAAECBJoWEEKabp/iCRAgQIBAuwJCSLu9UzkBAgQIEGhaQAhpun2KJ0CAAAEC7QoIIe32TuUECBAgQKBpASGk6fYpngABAgQItCsghLTbO5UTIECAAIGmBYSQptuneAIECBAg0K6AENJu71ROgAABAgSaFhBCmm6f4gkQIECAQLsCQki7vVM5AQIECBBoWkAIabp9iidAgAABAu0KCCHt9k7lBAgQIECgaQEhpOn2KZ4AAQIECLQrIIS02zuVEyBAgACBpgWEkKbbp3gCBAgQINCugBDSbu9UToAAAQIEmhYQQppun+IJECBAgEC7AkJIu71TOQECBAgQaFpACGm6fYonQIAAAQLtCggh7fZO5QQIECBAoGkBIaTp9imeAAECBAi0KyCEtNs7lRMgQIAAgaYFhJCm26d4AgQIECDQroAQ0m7vVE6AAAECBJoWEEKabp/iCRAgQIBAuwJCSLu9UzkBAgQIEGhaQAhpun2KJ0CAAAEC7QoIIe32TuUECBAgQKBpASGk6fYpngABAgQItCsghLTbO5UTIECAAIGmBYSQptuneAIECBAg0K6AENJu71ROgAABAgSaFhBCmm6f4gkQIECAQLsCQki7vVM5AQIECBBoWkAIabp9iidAgAABAu0KCCHt9k7lBAgQIECgaQEhpOn2KZ4AAQIECLQrIIS02zuVEyBAgACBpgWEkKbbp3gCBAgQINCugBDSbu9UToAAAQIEmhYQQppun+IJECBAgEC7AkJIu71TOQECBAgQaFpACGm6fYonQIAAAQLtCggh7fZO5QQIECBAoGkBIaTp9imeAAECBAi0KyCEtNs7lRMgQIAAgaYFhJCm26d4AgQIECDQroAQ0m7vVE6AAAECBJoWEEKabp/iCRAgQIBAuwJCSLu9UzkBAgQIEGhaQAhpun2KJ0CAAAEC7QoIIe32TuUECBAgQKBpASGk6fYpngABAgQItCsghLTbO5UTIECAAIGmBYSQptuneAIECBAg0K6AENJu71ROgAABAgSaFhBCmm6f4gkQIECAQLsCQki7vVM5AQIECBBoWkAIabp9iidAgAABAu0KCCHt9k7lBAgQIECgaQEhpOn2KZ4AAQIECLQrIIS02zuVEyBAgACBpgWEkKbbp3gCBAgQINCugBDSbu9UToAAAQIEmhYQQppun+IJECBAgEC7AkJIu71TOQECBAgQaFpACGm6fYonQIAAAQLtCggh7fZO5QQIECBAoGmB/w9GNuRerun8QgAAAABJRU5ErkJggg==";

        [TestMethod]
        public async Task CreateSuddenDeath()
        {
            var client = StartUp.GetClient();

            var sd = new SuddenDeath();
            sd.Id = Guid.NewGuid();
            sd.AreaLastSeenAlive = "tesco";
            sd.IncidentDate = DateTime.Now;
            sd.IncidentNumber = $"CP-{DateTime.Now.ToString("yyyyMMddHHmmss")}-1234";

            //text fields
            sd.BodyFoundBy = "fred";
            sd.BodyLabel = "body label";
            sd.BodyRemovedTo = "morgue";
            sd.CertifiedBy = "doctor";
            sd.DeathDiagnosedBy = "cat";
            sd.FamilyLiaisonOfficer = "cathy carter smith";
            sd.IdentificationLocation = "shop";
            sd.LastSeenAliveBy = "dog";
            sd.NextOfKinActionToInform = "phone call";
            sd.NextOfKinInformedMethod = "postcard";
            sd.PlaceOfDeath = "sainsburys";
            sd.UndertakerArrangingFuneral = "millers";
            sd.SupervisorNotes = "notes";

            var now = DateTime.Now;
            sd.IdentificationSignedOn = now;
            sd.DatetimeDeathConfirmed = now;
            sd.DatetimeBodyFound = now;
            sd.DatetimeLastSeenAlive = now;
            sd.AllPropertiesSignedOn = now;
            sd.InquestDate = now;

            sd.CIDCSIAttended = "Yes";
            sd.CIDCSIPhotosTaken = "Yes";
            sd.NextOfKinInformed = "Yes";
            sd.BodyIdentified = "Yes";
            sd.FormalIdentification = "Yes";
            sd.UndertakerRemovingBody = "Yes";
            sd.SuspectSuicide = "Yes";
            sd.DeathCertificateIssued = "Yes";

            sd.DeceasedAge = 51;

            sd.DOLS = "Yes";
            sd.DeathInHospital = "No";
            sd.SuicideNoteLeft = "Unknown";
            sd.IsSubmitted = "N/A";
            sd.Smoker = "Yes";
            sd.DeathInCustody = "Yes";
            sd.DeathInHealthCare = "Yes";
            sd.PoliceInvolvementPriorDeath = "Yes";
            sd.ApprovalStatus = "Rejected";

            sd.HouseTemperature = new Guid("46d79beb-5582-eb11-a812-000d3ade2967");
            sd.TPA = new Guid("83f1595a-245b-eb11-a812-0022489ba6ad");

            //sd.WorkRelatedDeath left as not set so we can check it gets set to null


            var postResp = await client.PostAsJsonAsync("api/suddendeath", sd).Result.Content.ReadAsStringAsync();
            var guid = JsonSerializer.Deserialize<Guid>(postResp);

            var checkSD = StartUp.adminService.GetEntity("cp_suddendeath", guid);
            ValidateSuddenDeath(sd, checkSD, now);

            var incident = StartUp.adminService.GetEntity(checkSD.GetEntityReferenceValue("cp_incident"));
            ValidateIncident(sd, incident,"Sudden Death", client.DefaultRequestHeaders.GetValues("UserEmail").Single());

        }

        private void ValidateSuddenDeath(SuddenDeath sd, Entity checkSD,DateTime checkDate)
        {
            Assert.AreEqual(sd.AreaLastSeenAlive, checkSD.GetValue<string>("cp_arealastseenalive"));
            Assert.AreEqual(sd.BodyFoundBy , checkSD.GetValue<string>("cp_bodyfoundby"));
            Assert.AreEqual(sd.BodyLabel , checkSD.GetValue<string>("cp_bodylabel"));
            Assert.AreEqual(sd.BodyRemovedTo , checkSD.GetValue<string>("cp_bodyremovedto"));
            Assert.AreEqual(sd.CertifiedBy , checkSD.GetValue<string>("cp_certifiedby"));
            Assert.AreEqual(sd.DeathDiagnosedBy , checkSD.GetValue<string>("cp_deathdiagnosedby"));
            Assert.AreEqual(sd.FamilyLiaisonOfficer , checkSD.GetValue<string>("cp_sdfamilylaisonofficer"));
            Assert.AreEqual(sd.IdentificationLocation , checkSD.GetValue<string>("cp_identificationlocation"));
            Assert.AreEqual(sd.LastSeenAliveBy , checkSD.GetValue<string>("cp_sdlastseenaliveby"));
            Assert.AreEqual(sd.NextOfKinActionToInform , checkSD.GetValue<string>("cp_nextofkinactiontoinform"));
            Assert.AreEqual(sd.NextOfKinInformedMethod , checkSD.GetValue<string>("cp_nextofkininformedmethod"));
            Assert.AreEqual(sd.PlaceOfDeath , checkSD.GetValue<string>("cp_placeofdeath"));
            Assert.AreEqual(sd.UndertakerArrangingFuneral, checkSD.GetValue<string>("cp_undertakerarrangingfuneral"));
            Assert.AreEqual(sd.SupervisorNotes, checkSD.GetValue<string>("cp_supervisornotes"));

            Assert.IsTrue(DateTimesMatch(checkDate, checkSD.GetValue<DateTime>("cp_identificationsignedon")));
            Assert.IsTrue(DateTimesMatch(checkDate, checkSD.GetValue<DateTime>("cp_datetimebodyfound")));
            Assert.IsTrue(DateTimesMatch(checkDate, checkSD.GetValue<DateTime>("cp_datetimedeathconfirmed")));
            Assert.IsTrue(DateTimesMatch(checkDate, checkSD.GetValue<DateTime>("cp_datetimelastseenalive")));
            Assert.IsTrue(DateTimesMatch(checkDate, checkSD.GetValue<DateTime>("cp_allpropertiessignedon")));
            Assert.IsTrue(DateTimesMatch(checkDate, checkSD.GetValue<DateTime>("cp_inquestdate")));

            Assert.IsTrue(checkSD.GetValue<bool>("cp_cidcsiattended"));
            Assert.IsTrue(checkSD.GetValue<bool>("cp_cidcsiphotostaken"));
            Assert.IsTrue(checkSD.GetValue<bool>("cp_nextofkininformed"));
            Assert.IsTrue(checkSD.GetValue<bool>("cp_bodyidentified"));
            Assert.IsTrue(checkSD.GetValue<bool>("cp_formalidentification"));
            Assert.IsTrue(checkSD.GetValue<bool>("cp_undertakerremovebody"));
            Assert.IsTrue(checkSD.GetValue<bool>("cp_suspectsuicide"));
            Assert.IsTrue(checkSD.GetValue<bool>("cp_deathcertificateissued"));

            Assert.AreEqual(51, checkSD.GetValue<int>("cp_deceasedage"));

            //sd.DOLS = "Yes";
            //sd.DeathInHospital = "No";
            //sd.SuicideNoteLeft = "Unknown";
            //sd.IsSubmitted = "N/A";
            //sd.Smoker = "Yes";
            //sd.DeathInCustody = "Yes";
            //sd.DeathInHealthCare = "Yes";
            //sd.PoliceInvolvementPriorDeath = "Yes";
            //sd.ApprovalStatus = "Rejected";
            Assert.AreEqual(778230000, checkSD.GetValue<OptionSetValue>("cp_dols").Value);
            Assert.AreEqual(778230001, checkSD.GetValue<OptionSetValue>("cp_deathinhospital").Value);
            Assert.AreEqual(778230002, checkSD.GetValue<OptionSetValue>("cp_suicidenoteleft_new").Value);
            Assert.AreEqual(778230002, checkSD.GetValue<OptionSetValue>("cp_issubmitted").Value);
            Assert.AreEqual(778230000, checkSD.GetValue<OptionSetValue>("cp_smoker").Value);
            Assert.AreEqual(778230000, checkSD.GetValue<OptionSetValue>("cp_deathincustody").Value);
            Assert.AreEqual(778230000, checkSD.GetValue<OptionSetValue>("cp_deathinhealthcare").Value);
            Assert.AreEqual(778230000, checkSD.GetValue<OptionSetValue>("cp_policeinvolvementpriordeath").Value);
            Assert.AreEqual(778230002, checkSD.GetValue<OptionSetValue>("cp_approvalstatus").Value);

            Assert.IsNull(checkSD.GetValue<OptionSetValue>("cp_workrelateddeath"));

            Assert.AreEqual(sd.HouseTemperature, checkSD.GetValue<EntityReference>("cp_housetemperature").Id);
            Assert.AreEqual(sd.TPA, checkSD.GetValue<EntityReference>("cp_tpa").Id);

        }


    }
}
